namespace Backend.Services.Implementations;

using Backend.Models.DTOs;
using Backend.Models.Entities;
using Backend.Data;
using Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

public class ProjectService : IProjectService
{
    private readonly AppDbContext _context;

    public ProjectService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Project>> GetAll()
    {
        return await _context.Projects
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Project>> GetAccessibleProjects(Guid userId, bool elevatedAccess)
    {
        var query = _context.Projects.AsNoTracking().AsQueryable();

        if (elevatedAccess)
            return await query.ToListAsync();

        return await query
            .Where(p => p.OwnerUserId == userId || _context.Tasks.Any(t => t.ProjectId == p.Id && t.AssignedUserId == userId))
            .ToListAsync();
    }

    public async Task<Project> Create(ProjectDto dto, Guid creatorUserId)
    {
        var project = new Project
        {
            Name = dto.Name,
            Description = dto.Description,
            OwnerUserId = creatorUserId
        };

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        return project;
    }

    public async Task<Project?> GetById(Guid id)
    {
        return await _context.Projects
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Project?> Update(Guid id, ProjectDto dto)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null)
            return null;

        project.Name = dto.Name;
        project.Description = dto.Description;

        await _context.SaveChangesAsync();
        return project;
    }

    public async Task<bool> Delete(Guid id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null)
            return false;

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ProjectExists(Guid id)
    {
        return await _context.Projects
            .AsNoTracking()
            .AnyAsync(p => p.Id == id);
    }

    public async Task<bool> HasReadAccess(Guid projectId, Guid userId, bool elevatedAccess)
    {
        if (elevatedAccess)
            return await ProjectExists(projectId);

        return await _context.Projects
            .AsNoTracking()
            .AnyAsync(p => p.Id == projectId &&
                           (p.OwnerUserId == userId ||
                            _context.Tasks.Any(t => t.ProjectId == p.Id && t.AssignedUserId == userId)));
    }

    public async Task<bool> HasWriteAccess(Guid projectId, Guid userId, bool elevatedAccess)
    {
        if (elevatedAccess)
            return await ProjectExists(projectId);

        return await _context.Projects
            .AsNoTracking()
            .AnyAsync(p => p.Id == projectId && p.OwnerUserId == userId);
    }
}
