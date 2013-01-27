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
				.WithColumn("TrainerId").AsInt32().Nullable()
					      
				.WithColumn("IsDeleted").AsBoolean().Nullable()
                .WithColumn("CompanyId").AsInt32().Nullable()
				.WithColumn("CreatedById").AsInt32().Nullable()
                .WithColumn("ChangedById").AsInt32().Nullable()
				.WithColumn("ChangedDate").AsDateTime().Nullable()
                .WithColumn("CreatedDate").AsDateTime().Nullable()
                //don't know why I need this but NH creates it so wtf
                .WithColumn("UserId").AsInt32().Nullable();


            Create.ForeignKey("FK_TrainerSessionVerification_manyToOne_TrainerId").FromTable("TrainerSessionVerification").InSchema("dbo").ForeignColumns("TrainerId").ToTable("User").InSchema("dbo").PrimaryColumns("EntityId");
            Create.ForeignKey("FK_TrainerSessionVerification_manyToOne_User_Created").FromTable("TrainerSessionVerification").InSchema("dbo").ForeignColumns("CreatedById").ToTable("User").InSchema("dbo").PrimaryColumns("EntityId");
            Create.ForeignKey("FK_TrainerSessionVerification_manyToOne_User_Changed").FromTable("TrainerSessionVerification").InSchema("dbo").ForeignColumns("ChangedById").ToTable("User").InSchema("dbo").PrimaryColumns("EntityId");
            //don't know why I need this but NH creates it so wtf
            Create.ForeignKey("FK_TrainerSessionVerification_oneToMany_User").FromTable("TrainerSessionVerification").InSchema("dbo").ForeignColumns("UserId").ToTable("User").InSchema("dbo").PrimaryColumns("EntityId");
        }

        public override void Down()
        {         
			Delete.Table("TrainerSessionVerification");
        }
    }
}