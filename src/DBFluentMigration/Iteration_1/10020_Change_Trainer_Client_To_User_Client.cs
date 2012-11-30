using FluentMigrator;

namespace DBFluentMigration.Iteration_1
{
     [Migration(10020)]
    public class Change_Trainer_Client_To_User_Client:Migration
    {
         public override void Up()
         {
             Rename.Table("Trainer_Client").To("User_Client");
             // not commited yet so use old name
             Rename.Column("TrainerId").OnTable("User_Client").To("UserId");
         }

         public override void Down()
         {
             Rename.Table("User_Client").To("Trainer_Client");
             Rename.Column("UserId").OnTable("User_Client").To("TrainerId");
         }
    }
}