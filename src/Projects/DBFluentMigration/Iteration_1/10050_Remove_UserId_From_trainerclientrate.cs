using FluentMigrator;

namespace DBFluentMigration.Iteration_1
{
    [Migration(10050)]
    public class Remove_UserId_From_trainerclientrate : Migration
    {
        public override void Up()
        {
            Delete.ForeignKey("FK_TrainerClientRate_manyToOne_User").OnTable("TrainerClientRate");
            Delete.Column("UserId").FromTable("TrainerClientRate");
        }

        public override void Down()
        {
            Create.Column("UserId").OnTable("TrainerClientRate").AsInt32().Nullable();
        }
    }
}