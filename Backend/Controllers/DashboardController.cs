namespace Backend.Controllers;

using Microsoft.AspNetCore.Mvc;
using Backend.Services.Interfaces;

[ApiController]
[Route("api/dashboard")]
// [Authorize] - disabled
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _service;

    public DashboardController(IDashboardService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetStats()
    {
        return Ok(await _service.GetDashboardStats());
    }
}