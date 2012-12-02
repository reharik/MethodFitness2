using FluentMigrator;

namespace DBFluentMigration.Iteration_1
{
    [Migration(10040)]
    public class Add_Status_To_User : Migration
    {
        public override void Up()
        {
            Alter.Table("User").AddColumn("Status").AsString(50).WithDefaultValue("Active").NotNullable();
        }

        public override void Down()
        {
            Delete.Column("Status").FromTable("User");
        }
    }
}