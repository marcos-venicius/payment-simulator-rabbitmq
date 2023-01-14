using Core.Infra.Context;
using Core.RabbitMq.Models;
using Core.Repositories;
using Core.Repositories.Interfaces;
using Core.Services;
using Core.Services.Interfaces;

namespace Core.Infra.IoC;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IDbContext, DapperContext>();
        services.AddSingleton<Database>();

        services.AddSingleton<IMessageProducerService<PaymentData>, RabbitMqPaymentProducerService>();

        services.AddTransient<IPaymentRepository, PaymentRepository>();
        services.AddTransient<IPaymentService, PaymentService>();
        
        return services;
    }
}