using FluentMigrator;

namespace DBFluentMigration.Iteration_1
{
    [Migration(10040)]
    public class Add_columns_and_FKs_To_Session : Migration
    {
        public override void Up()
        {
            Create.Column("TrainerVerified").OnTable("Session").AsBoolean().WithDefaultValue("False");
            Create.Column("TrainerSessionVerificationId").OnTable("Session").AsInt32().Nullable();
            Create.Column("UserId").OnTable("Session").AsInt32().Nullable();
            Create.ForeignKey("FK_Session_manyToOne_Trainer").FromTable("Session").ForeignColumn("TrainerId").ToTable("User").PrimaryColumn("EntityId");
            Create.ForeignKey("FK_Sessions_oneToMany_User").FromTable("Session").ForeignColumn("UserId").ToTable("User").PrimaryColumn("EntityId");
            Create.ForeignKey("FK_TrainerApprovedSessionItems_oneToMany_TrainerSessionVerification").FromTable("Session").ForeignColumn("TrainerSessionVerificationid").ToTable("TrainerSessionVerification").PrimaryColumn("EntityId");
        }

        public override void Down()
        {
            Delete.Column("TrainerVerified").FromTable("Session");
        }
    }
}