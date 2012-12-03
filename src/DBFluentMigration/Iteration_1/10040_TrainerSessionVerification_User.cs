using FluentMigrator;

namespace DBFluentMigration.Iteration_1
{
    [Migration(10040)]
    public class TrainerSessionVerification_User : Migration
    {
        public override void Up()
        {
            Create.Table("TrainerSessionVerification_User").InSchema("dbo")
                  .WithColumn("TrainerSessionVerification_id").AsInt32().PrimaryKey().NotNullable()
                  .WithColumn("User_id").AsInt32().PrimaryKey().NotNullable();

            Create.ForeignKey("FK_TrainerSessionVerification_manyToMany_User")
                  .FromTable("TrainerSessionVerification_User")
                  .InSchema("dbo")
                  .ForeignColumns("TrainerSessionVerification_id")
                  .ToTable("TrainerSessionVerification")
                  .InSchema("dbo")
                  .PrimaryColumns("EntityId");
            Create.ForeignKey("FK_User_manyToMany_TrainerSessionVerification")
                  .FromTable("TrainerSessionVerification_User")
                  .InSchema("dbo")
                  .ForeignColumns("User_id")
                  .ToTable("User")
                  .InSchema("dbo")
                  .PrimaryColumns("EntityId");
        }

        public override void Down()
        {
            Delete.Table("TrainerSessionVerification_User");
        }
    }
}