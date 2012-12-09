@ECHO OFF
src\DBFluentMigration\MigrateApp\migrate -a src\DBFluentMigration\bin\debug\DBFluentMigration.dll -db sqlserver2008 -conn "Data Source=(local);Initial Catalog=MethodFitness_DEV;Integrated Security=True;"
echo "Done!"
