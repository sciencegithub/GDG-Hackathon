namespace Backend.Services.Implementations;

using Backend.Models.DTOs;
using Backend.Data;
using Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _context;

    public DashboardService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardStatsDto> GetDashboardStats()
    {
        var now = DateTime.UtcNow;

        // Get all tasks (excluding soft deleted)
        var allTasks = await _context.Tasks
            .AsNoTracking()
            .ToListAsync();

        // Get all users
        var allUsers = await _context.Users
            .AsNoTracking()
            .ToListAsync();

        // Calculate basic stats
        var totalTasks = allTasks.Count;
        var totalUsers = allUsers.Count;
        var activeTasks = allTasks.Count(t => t.Status != "Done");
        var completedTasks = allTasks.Count(t => t.Status == "Done");
        var overdueTasks = allTasks.Count(t => t.DueDate.HasValue && t.DueDate.Value < now && t.Status != "Done");

        // Tasks by status
        var tasksByStatus = allTasks
            .GroupBy(t => t.Status)
            .ToDictionary(g => g.Key, g => g.Count());

        // Tasks per user
        var tasksPerUser = new List<UserTaskStatsDto>();

        foreach (var user in allUsers)
        {
            var userTasks = allTasks.Where(t => t.AssignedUserId == user.Id).ToList();
            var userActiveTasks = userTasks.Count(t => t.Status != "Done");
            var userCompletedTasks = userTasks.Count(t => t.Status == "Done");
            var userOverdueTasks = userTasks.Count(t => t.DueDate.HasValue && t.DueDate.Value < now && t.Status != "Done");

            tasksPerUser.Add(new UserTaskStatsDto
            {
                UserId = user.Id,
                UserName = user.Name,
                TotalTasks = userTasks.Count,
                ActiveTasks = userActiveTasks,
                CompletedTasks = userCompletedTasks,
                OverdueTasks = userOverdueTasks
            });
        }

        return new DashboardStatsDto
        {
            TotalTasks = totalTasks,
            TotalUsers = totalUsers,
            ActiveTasks = activeTasks,
            CompletedTasks = completedTasks,
            OverdueTasks = overdueTasks,
            TasksByStatus = tasksByStatus,
            TasksPerUser = tasksPerUser
        };
    }
}