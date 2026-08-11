namespace TodoApp.Domain.Interfaces;

public interface ISoftDeletable
{
    DateTime? DeletedAt { get; set; }
}
