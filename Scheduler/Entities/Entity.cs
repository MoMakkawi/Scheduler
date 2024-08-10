namespace Scheduler.Entities;

public class Entity
{
    public int Id { get; set; }
    public DateTime? ExecutedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime CreatedAt { get; private set; } = DateTime.Now;
    public bool IsDeleted { get; set; } = false;
}
