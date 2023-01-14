namespace PaymentService.Delegates;

public delegate Task ConsumerEvent<in T>(T data);
