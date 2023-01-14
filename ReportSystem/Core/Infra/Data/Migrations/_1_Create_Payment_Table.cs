using FluentMigrator;

namespace Core.Infra.Data.Migrations;

[Migration(1)]
public sealed class _1_Create_Payment_Table : Migration
{
    private const string TableName = "Payment";

    public override void Up()
    {
        Create.Table(TableName)
            .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
            .WithColumn("Name").AsString().NotNullable()
            .WithColumn("Price").AsInt32().NotNullable()
            .WithColumn("State").AsString().NotNullable().WithDefaultValue("processing")
            .WithColumn("CreatedAt").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);
    }

    public override void Down()
    {
        Delete.Table(TableName);
    }
}