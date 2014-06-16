using FluentMigrator;

namespace DBFluentMigration.Iteration_1
{
    [Migration(10010)]
    public class Drop_User_Type_Column_From_UserTable : Migration
    {
        public override void Up()
        {
            Delete.Column("Type").FromTable("User");
        }

        public override void Down()
        {
            Create.Column("Type").OnTable("User").AsString().NotNullable();
        }
    }
}