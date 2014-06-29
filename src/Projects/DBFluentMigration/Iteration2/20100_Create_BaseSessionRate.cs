using FluentMigrator;

namespace Migrations
{
    [Migration(20100)]
    public class Create_BaseSessionRate : Migration
    {
        public override void Up()
        {
            Create.Table("BaseSessionRate")
                  .WithColumn("FullHour").AsDouble()
                  .WithColumn("HalfHour").AsDouble()
                  .WithColumn("FullHourTenPack").AsDouble()
                  .WithColumn("HalfHourTenPack").AsDouble()
                  .WithColumn("Pair").AsDouble()
                  .WithColumn("PairTenPack").AsDouble()
                  .WithColumn("EntityId").AsInt32().PrimaryKey().Identity()
                  .WithColumn("CompanyId").AsInt32()
                  .WithColumn("CreatedById").AsInt32().ForeignKey("User", "EntityId")
                  .WithColumn("ChangedById").AsInt32().ForeignKey("User","EntityId")
                  .WithColumn("CreatedDate").AsDateTime()
                  .WithColumn("ChangedDate").AsDateTime()
                  .WithColumn("IsDeleted").AsBoolean();
        }

        public override void Down()
        {
            
        }
    }
}


