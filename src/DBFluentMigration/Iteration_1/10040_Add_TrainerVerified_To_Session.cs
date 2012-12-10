using FluentMigrator;

namespace DBFluentMigration.Iteration_1
{
    [Migration(10040)]
    public class Add_TrainerVerified_To_Session : Migration
    {
        public override void Up()
        {
            Create.Column("TrainerVerified").OnTable("Session").AsBoolean().WithDefaultValue("False");
        }

        public override void Down()
        {
            Delete.Column("TrainerVerified").FromTable("Session");
        }
    }
}