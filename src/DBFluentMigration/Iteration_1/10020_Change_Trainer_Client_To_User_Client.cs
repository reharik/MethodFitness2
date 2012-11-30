using FluentMigrator;

namespace DBFluentMigration.Iteration_1
{
     [Migration(10020)]
    public class Change_Trainer_All_Columns_and_Tables_From_Trainer_To_User:Migration
    {
         public override void Up()
         {
//             Rename.Table("Trainer_Client").To("User_Client");
//             Rename.Column("TrainerId").OnTable("User_Client").To("UserId");
//             Rename.Column("TrainerId").OnTable("Appointment").To("UserId");
             Rename.Column("TrainerId").OnTable("Session").To("UserId");
////             Rename.Column("TrainerId").OnTable("TrainerClientRate").To("UserId");
//             Rename.Column("TrainerId").OnTable("TrainerPayment").To("UserId");
         }

         public override void Down()
         {
             Rename.Table("User_Client").To("Trainer_Client");
             Rename.Column("UserId").OnTable("User_Client").To("TrainerId");
             Rename.Column("UserId").OnTable("Appointment").To("TrainerId");
             Rename.Column("UserId").OnTable("Session").To("TrainerId");
             Rename.Column("UserId").OnTable("TrainerClientRate").To("TrainerId");
             Rename.Column("UserId").OnTable("TrainerPayment").To("TrainerId");
         }
    }
}