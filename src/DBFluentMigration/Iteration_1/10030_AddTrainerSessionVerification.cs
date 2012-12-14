using FluentMigrator;

namespace DefaultNamespace
{
    [Migration(10030)]
    public class AddTrainerSessionVerification : Migration
    {
        public override void Up()
        {      
			Create.Table("TrainerSessionVerification").InSchema("dbo")
               .WithColumn("EntityId").AsInt32().PrimaryKey().Identity().NotNullable()
				.WithColumn("Total").AsDouble().Nullable()
				.WithColumn("TrainerId").AsInt32().NotNullable()
					      
				.WithColumn("IsDeleted").AsBoolean().Nullable()
                .WithColumn("CompanyId").AsInt32().Nullable()
				.WithColumn("CreatedById").AsInt32().Nullable()
                .WithColumn("ChangedById").AsInt32().Nullable()
				.WithColumn("ChangedDate").AsDateTime().Nullable()
                .WithColumn("CreatedDate").AsDateTime().Nullable();

            Create.ForeignKey("FK_TrainerSessionVerification_oneToMany_TrainerId").FromTable("TrainerSessionVerification").InSchema("dbo").ForeignColumns("TrainerId").ToTable("User").InSchema("dbo").PrimaryColumns("EntityId");
            Create.ForeignKey("FK_TrainerSessionVerification_oneToMany_User_Created").FromTable("TrainerSessionVerification").InSchema("dbo").ForeignColumns("CreatedById").ToTable("User").InSchema("dbo").PrimaryColumns("EntityId");
            Create.ForeignKey("FK_TrainerSessionVerification_oneToMany_User_Changed").FromTable("TrainerSessionVerification").InSchema("dbo").ForeignColumns("ChangedById").ToTable("User").InSchema("dbo").PrimaryColumns("EntityId");
        }

        public override void Down()
        {         
			Delete.Table("TrainerSessionVerification");
        }
    }
}