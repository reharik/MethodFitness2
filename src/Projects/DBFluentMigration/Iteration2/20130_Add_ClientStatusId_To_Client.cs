using FluentMigrator;
using FluentMigrator;

namespace DefaultNamespace
{
    [Migration(20130)]
    public class Add_ClientStatusId_To_Client : Migration
    {
        public override void Up()
        {
            Create.Column("ClientStatusId").OnTable("Client").AsInt32();
        }

        public override void Down()
        {
        }
    }
}
