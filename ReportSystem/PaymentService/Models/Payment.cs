namespace PaymentService.Models;

public class Payment
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public int Price { get; set; }
    public string State { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}