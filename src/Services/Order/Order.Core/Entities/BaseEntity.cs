namespace Order.Core.Entities;

public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    
    public BaseEntity()
    {
        CreatedDate = DateTime.UtcNow;
    }
} 