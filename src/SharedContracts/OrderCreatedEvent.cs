namespace SharedContracts;

public record OrderCreatedEvent
{
    public Guid OrderId { get; init; } = Guid.NewGuid();
    public string CustomerEmail { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}