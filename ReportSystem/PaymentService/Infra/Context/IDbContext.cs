using System.Data;

namespace PaymentService.Infra.Context;

public interface IDbContext
{
    IDbConnection CreateConnection();
}