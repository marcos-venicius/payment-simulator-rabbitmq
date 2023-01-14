namespace Core.Services.Requests.PaymentServiceRequests;

public sealed record CreatePaymentRequest(string Name, int Price);
