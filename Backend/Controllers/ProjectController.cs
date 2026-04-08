namespace Backend.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Backend.Models.DTOs;
using Backend.Services.Interfaces;
using System.Security.Claims;

[ApiController]
[Route("api/projects")]
[Authorize]
public class ProjectController : ControllerBase
{
    private readonly IProjectService _service;

    public ProjectController(IProjectService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Policy = "ProjectRead")]
    public async Task<IActionResult> GetAll()
    {
        var projects = await _service.GetAccessibleProjects(GetCurrentUserId(), HasElevatedAccess());
        return Ok(ApiResponseDto<List<Backend.Models.Entities.Project>>.Ok(projects, "Projects retrieved"));
    }

    [HttpPost]
    [Authorize(Policy = "ProjectWrite")]
    public async Task<IActionResult> Create([FromBody] ProjectDto dto)
    {
        var project = await _service.Create(dto, GetCurrentUserId());
        return Ok(ApiResponseDto<Backend.Models.Entities.Project>.Ok(project, "Project created"));
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "ProjectRead")]
    public async Task<IActionResult> GetById(Guid id)
    {
        if (!await _service.ProjectExists(id))
            return NotFound(ApiResponseDto<object>.Fail("Project not found"));

        var canRead = await _service.HasReadAccess(id, GetCurrentUserId(), HasElevatedAccess());
        if (!canRead)
            return Forbid();

        var project = await _service.GetById(id);
        if (project == null)
            return NotFound(ApiResponseDto<object>.Fail("Project not found"));

        return Ok(ApiResponseDto<Backend.Models.Entities.Project>.Ok(project, "Project retrieved"));
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "ProjectWrite")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ProjectDto dto)
    {
        if (!await _service.ProjectExists(id))
            return NotFound(ApiResponseDto<object>.Fail("Project not found"));

        var canWrite = await _service.HasWriteAccess(id, GetCurrentUserId(), HasElevatedAccess());
        if (!canWrite)
            return Forbid();

        var project = await _service.Update(id, dto);
        if (project == null)
            return NotFound(ApiResponseDto<object>.Fail("Project not found"));

        return Ok(ApiResponseDto<Backend.Models.Entities.Project>.Ok(project, "Project updated"));
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "ProjectWrite")]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (!await _service.ProjectExists(id))
            return NotFound(ApiResponseDto<object>.Fail("Project not found"));

        var canWrite = await _service.HasWriteAccess(id, GetCurrentUserId(), HasElevatedAccess());
        if (!canWrite)
            return Forbid();

        var result = await _service.Delete(id);
        if (!result)
            return NotFound(ApiResponseDto<object>.Fail("Project not found"));

        return Ok(ApiResponseDto<object>.Ok(null, "Project deleted"));
    }

    private bool HasElevatedAccess()
    {
        return User.IsInRole("Admin") || User.IsInRole("Manager");
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("Invalid user context");

        return userId;
    }
}
