using Dapper;
using PaymentService.Infra.Context;
using PaymentService.RabbitMq.Models;
using PaymentService.Repositories.Interfaces;

namespace PaymentService.Repositories;

public sealed class PaymentRepository : IPaymentRepository
{
    private readonly IDbContext _context;

    public PaymentRepository(IDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        using var connection = _context.CreateConnection();

        var item = await connection.QueryFirstOrDefaultAsync<object>("SELECT TOP(1) * FROM Payment WHERE Id = @Id", new
        {
            Id = id
        });

        return item is not null;
    }

    public async Task SaveAsync(PaymentData data)
    {
        using var connection = _context.CreateConnection();

        await connection.ExecuteAsync(
            sql: "INSERT INTO Payment (Id, Name, Price) VALUES (@Id, @Name, @Price);",
            param: data
        );
    }

    public async Task UpdateStatusAsync(Guid id, string status)
    {
        using var connection = _context.CreateConnection();

        await connection.ExecuteAsync(
            sql: "UPDATE Payment SET State = @State WHERE Id = @Id",
            param: new
            {
                Id = id,
                State = status
            }
        );
    }
}