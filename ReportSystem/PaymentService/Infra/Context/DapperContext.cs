using System.Data;
using System.Data.SqlClient;

namespace PaymentService.Infra.Context;

public sealed class DapperContext : IDbContext
{
    private readonly IConfiguration _configuration;

    public DapperContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IDbConnection CreateConnection() => new SqlConnection(_configuration.GetConnectionString("sqlserver"));
}