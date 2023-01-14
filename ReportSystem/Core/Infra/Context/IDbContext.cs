using System.Data;

namespace Core.Infra.Context;

public interface IDbContext
{
    IDbConnection CreateConnection();
    IDbConnection CreateMasterConnection();
}