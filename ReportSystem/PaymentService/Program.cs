using PaymentService;
using PaymentService.Infra.Context;
using PaymentService.RabbitMq.Models;
using PaymentService.Repositories;
using PaymentService.Repositories.Interfaces;
using PaymentService.Services;
using PaymentService.Services.Interfaces;
using PaymentServiceClass = PaymentService.Services.PaymentService;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<IDbContext, DapperContext>();
        services.AddSingleton<IMessageConsumerService<PaymentData>, RabbitMqPaymentConsumerService>();

        services.AddTransient<IPaymentRepository, PaymentRepository>();
        services.AddTransient<IPaymentService, PaymentServiceClass>();

        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();