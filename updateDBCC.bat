@ECHO OFF
src\DBFluentMigration\MigrateApp\migrate -a src\DBFluentMigration\bin\debug\DBFluentMigration.dll -db sqlserver2008 -conn "Server=cannibalcoder.cloudapp.net;Database=MethodFitness_QA;User ID=methodfitness;Password=m3th0d;Connection Timeout=30;"
echo "Done!"
