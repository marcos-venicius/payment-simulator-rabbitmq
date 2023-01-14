using Core.Infra.Context;
using Dapper;

namespace Core.Infra;

public class Database
{
    private readonly IDbContext _context;

    public Database(IDbContext context)
    {
        _context = context;
    }

    public void CreateDatabaseIfNotExists(string databaseName)
    {
        const string findDatabasesQuery = "SELECT * FROM sys.databases WHERE name = @name";

        var findDatabasesQueryParams = new DynamicParameters();
        
        findDatabasesQueryParams.Add("name", databaseName);

        using var connection = _context.CreateMasterConnection();
        
        var databases = connection.Query(findDatabasesQuery, findDatabasesQueryParams);
        
        if (!databases.Any())
            connection.Execute($"CREATE DATABASE {databaseName}");
    }
}