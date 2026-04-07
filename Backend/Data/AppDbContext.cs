namespace Backend.Data;

using Microsoft.EntityFrameworkCore;
using Backend.Models.Entities;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<TaskItem> Tasks { get; set; }
    public DbSet<TaskComment> Comments { get; set; }
    public DbSet<ChecklistItem> ChecklistItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TaskItem>().HasQueryFilter(task => !task.IsDeleted);
        modelBuilder.Entity<TaskComment>().HasQueryFilter(comment => !comment.IsDeleted);
        modelBuilder.Entity<ChecklistItem>().HasQueryFilter(item => !item.IsDeleted);

        // Foreign key relationships
        modelBuilder.Entity<TaskComment>()
            .HasOne(tc => tc.Task)
            .WithMany()
            .HasForeignKey(tc => tc.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TaskComment>()
            .HasOne(tc => tc.Author)
            .WithMany()
            .HasForeignKey(tc => tc.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ChecklistItem>()
            .HasOne(ci => ci.Task)
            .WithMany()
            .HasForeignKey(ci => ci.TaskId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
