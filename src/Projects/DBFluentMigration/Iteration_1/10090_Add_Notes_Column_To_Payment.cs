using FluentMigrator;

namespace DefaultNamespace
{
    [Migration(10090)]
    public class Add_Notes_Column_To_Payment : Migration
    {
        public override void Up()
        {
            Create.Column("Notes").OnTable("Payment").AsString().Nullable();
        }

        public override void Down()
        {         
			Delete.Column("Notes").FromTable("Payment");
        }
    }
}