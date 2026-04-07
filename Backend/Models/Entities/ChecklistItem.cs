namespace Backend.Models.Entities;

public class ChecklistItem
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public bool IsCompleted { get; set; } = false;

    public int Order { get; set; } = 0; // To maintain order of checklist items

    public Guid TaskId { get; set; }
    public TaskItem Task { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }

    public bool IsDeleted { get; set; } = false; // soft delete
}
