using FluentMigrator;
using FluentMigrator;

namespace DefaultNamespace
{
    [Migration(20120)]
    public class Add_ClientStatus_Table : Migration
    {
        public override void Up()
        {
            Create.Table("ClientStatus")
                  .WithColumn("AdminAlerted").AsBoolean()
                  .WithColumn("ClientContacted").AsBoolean()
                  .WithColumn("EntityId").AsInt32().PrimaryKey().Identity()
                  .WithColumn("CreatedBy").AsInt32()
                  .WithColumn("CreatedDate").AsDateTime()
                  .WithColumn("ChangedBy").AsInt32()
                  .WithColumn("ChangedDate").AsDateTime()
                  .WithColumn("CompanyId").AsInt32()
                  .WithColumn("IsDeleted").AsBoolean();
        }

        public override void Down()
        {
        }
    }
}
