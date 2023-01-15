using Core.Infra.Context;
using Core.Models;
using Core.RabbitMq.Models;
using Core.Repositories.Interfaces;
using Dapper;

namespace Core.Repositories;

public sealed class PaymentRepository : IPaymentRepository
{
    private readonly IDbContext _context;

    public PaymentRepository(IDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Payment>> ListAsync()
    {
        using var connection = _context.CreateConnection();

        return await connection.QueryAsync<Payment>("SELECT * FROM Payment ORDER BY CreatedAt DESC");
    }

    public async Task SaveAsync(PaymentData data)
    {
        using var connection = _context.CreateConnection();

        await connection.ExecuteAsync(
            "INSERT INTO Payment (Id, Name, Price, State) VALUES (@Id, @Name, @Price, 'waiting')",
            data
        );
    }
}