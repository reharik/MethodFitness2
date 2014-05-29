using FluentMigrator;
using FluentMigrator;

namespace DefaultNamespace
{
    [Migration(20110)]
    public class Add_IsActive_Column_To_Users_and_clients : Migration
    {
        public override void Up()
        {
            Create.Column("InActive").OnTable("User").AsBoolean();
            Create.Column("InActive").OnTable("Client").AsBoolean();
        }

        public override void Down()
        {
            Delete.Column("InActive").FromTable("User");
            Delete.Column("InActive").FromTable("Client");
        }
    }
}