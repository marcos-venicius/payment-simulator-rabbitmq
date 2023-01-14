using System.Reflection;
using Core.Infra;
using Core.Infra.IoC;
using FluentMigrator.Runner;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(c => c.AddFluentMigratorConsole())
    .AddFluentMigratorCore()
    .ConfigureRunner(c => c.AddSqlServer2012()
        .WithGlobalConnectionString(builder.Configuration.GetConnectionString("sqlserver"))
        .ScanIn(Assembly.GetExecutingAssembly()).For.Migrations());

builder.Services.AddInfrastructure();

builder.Services.AddControllers();
builder.Services.AddCors(options => options
    .AddPolicy(
        "ClientPermission",
        policy => policy
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithOrigins("http://localhost:3000")
            .AllowCredentials()
    )
);

var app = builder.Build();

app.UseRouting();

app.UseCors("ClientPermission");

app.MapControllers();

app.RunMigrations().Run();