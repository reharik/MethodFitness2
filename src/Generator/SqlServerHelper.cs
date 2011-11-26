using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using NHibernate;

namespace Generator
{
    public static class SqlServerHelper
    {
        public static void KillAllFKs()
        {
            var conStr = ConfigurationSettings.AppSettings["MethodFitness.sql_server_connection_string"];
            IDbConnection conn = new SqlConnection(conStr);
            using (conn)
            {
                try
                {
                    var sql = @"

DECLARE @tablename sysname, @constraint sysname
DECLARE FK_KILLER CURSOR FOR 
SELECT fk.table_name, c.constraint_name 
  FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS C 
  JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS FK ON C.CONSTRAINT_NAME = FK.CONSTRAINT_NAME

OPEN FK_KILLER
FETCH NEXT FROM FK_KILLER INTO @tablename, @constraint
WHILE @@FETCH_STATUS = 0
BEGIN
	SELECT 'alter table [' + @tablename + '] drop constraint ' + @constraint
	EXECUTE ('alter table [' + @tablename + '] drop constraint ' + @constraint + ';')
	FETCH NEXT FROM FK_KILLER INTO @tablename, @constraint
END
CLOSE FK_KILLER
DEALLOCATE FK_KILLER
";

                    conn.Open();
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    conn.Close();
                    Console.WriteLine(e);
                }
            }
        }

        public static void GenerateSecurityTables(ISessionFactory source)
        {
            using (ISession session = source.OpenSession())
            {
                try
                {
                    var rhinoSecurityScript = new System.IO.StreamReader("src\\new_rhinosecurity.sql");
                    string sql = rhinoSecurityScript.ReadToEnd();
                    rhinoSecurityScript.Close();
                    IDbConnection conn = session.Connection;
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}