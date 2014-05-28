using FluentMigrator;
using FluentMigrator;

namespace DefaultNamespace
{
    [Migration(20110)]
    public class Add_Archived_Column_To_Users_and_clients : Migration
    {
        public override void Up()
        {
            Create.Column("Archived").OnTable("User").AsBoolean().WithDefaultValue(false);
            Create.Column("Archived").OnTable("Client").AsBoolean().WithDefaultValue(false);
        }

        public override void Down()
        {
            Delete.Column("Archived").FromTable("User");
            Delete.Column("Archived").FromTable("Client");
        }
    }
}