﻿using FluentMigrator.Runner;

namespace Core.Infra;

public static class MigrationManager
{
    public static IHost RunMigrations(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        
        var databaseService = scope.ServiceProvider.GetRequiredService<Database>();
        var migrationService = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        
        databaseService.CreateDatabaseIfNotExists("ReportSystem");
        
        migrationService.ListMigrations();
        migrationService.MigrateUp();
        
        return host;
    }
}