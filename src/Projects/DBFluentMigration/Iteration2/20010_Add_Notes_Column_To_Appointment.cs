using FluentMigrator;

namespace DefaultNamespace
{
    [Migration(20010)]
    public class Add_Notes_Column_To_Appointment : Migration
    {
        public override void Up()
        {
            Create.Column("Notes").OnTable("Appointment").AsString().Nullable();
        }

        public override void Down()
        {         
			Delete.Column("Notes").FromTable("Appointment");
        }
    }
}