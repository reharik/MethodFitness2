using FluentMigrator;

namespace DBFluentMigration.Iteration_1
{
    [Migration(10030)]
    public class Add_Status_To_Client : Migration
    {
        public override void Up()
        {
            Alter.Table("Client").AddColumn("Status").AsString(50).WithDefaultValue("Active").NotNullable();
        }

        public override void Down()
        {
            Delete.Column("Status").FromTable("Client");
        }
    }
}