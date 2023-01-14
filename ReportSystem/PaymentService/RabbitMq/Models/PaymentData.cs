namespace PaymentService.RabbitMq.Models;

public sealed record PaymentData(Guid Id, string Name, int Price);
