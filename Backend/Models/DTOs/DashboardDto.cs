namespace Backend.Models.DTOs;

public class DashboardStatsDto
{
    public int TotalTasks { get; set; }
    public int TotalUsers { get; set; }
    public int ActiveTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int OverdueTasks { get; set; }
    public List<UserTaskStatsDto> TasksPerUser { get; set; } = new();
    public Dictionary<string, int> TasksByStatus { get; set; } = new();
}

public class UserTaskStatsDto
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int TotalTasks { get; set; }
    public int ActiveTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int OverdueTasks { get; set; }
}