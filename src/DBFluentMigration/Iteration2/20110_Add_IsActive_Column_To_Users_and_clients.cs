using FluentMigrator;
using FluentMigrator;

namespace DefaultNamespace
{
    [Migration(20110)]
    public class Add_IsActive_Column_To_Users_and_clients : Migration
    {
        public override void Up()
        {
            Create.Column("IsActive").OnTable("User").AsBoolean();
            Create.Column("IsActive").OnTable("Client").AsBoolean();
        }

        public override void Down()
        {
            Delete.Column("IsActive").FromTable("User");
            Delete.Column("IsActive").FromTable("Client");
        }
    }
}