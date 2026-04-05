namespace Backend.Services.Interfaces;

using Backend.Models.DTOs;

public interface IDashboardService
{
    Task<DashboardStatsDto> GetDashboardStats();
}