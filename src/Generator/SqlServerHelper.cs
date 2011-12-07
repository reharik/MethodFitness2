using System;
using System.Data;
using System.IO;
using MethodFitness.Core.Config;
using NHibernate;

namespace Generator
{
    public static class SqlServerHelper
    {
        public static void KillAllFKs(ISessionFactory source)
        {
            using (ISession session = source.OpenSession(new SaveUpdateInterceptor()))
            {
                try
                {
                    var sql =
                        @"declare @tablename sysname, @constraint sysname
declare FK_KILLER CURSOR FOR 
SELECT fk.table_name, c.constraint_name 
FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS C 
JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS FK ON C.CONSTRAINT_NAME = FK.CONSTRAINT_NAME
OPEN FK_KILLER
FETCH NEXT FROM FK_KILLER INTO @tablename, @constraint
WHILE @@FETCH_STATUS = 0
BEGIN
	SELECT 'alter table ' + @tablename + ' drop constraint ' + @constraint
	EXECUTE ('alter table ' + @tablename + ' drop constraint ' + @constraint + ';')
	FETCH NEXT FROM FK_KILLER INTO @tablename, @constraint
END
CLOSE FK_KILLER
DEALLOCATE FK_KILLER
";

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

        public static void DeleteReaddDb(ISessionFactory source)
        {
            using (ISession session = source.OpenSession(new SaveUpdateInterceptor()))
            {
                var sql =
                    "USE [master] alter database methodfitness_DEV set single_user with rollback immediate DROP DATABASE methodfitness_DEV CREATE DATABASE methodfitness_DEV";

                IDbConnection conn = session.Connection;
                var cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }
        }

        public static void AddRhinoSecurity(ISessionFactory source)
        {
            var exePath = AppDomain.CurrentDomain.BaseDirectory;
            var newPath = exePath + @"sqlscripts\new_rhinosecurity.sql";
            var rhino = new StreamReader(newPath);

            var sql = rhino.ReadToEnd();
            rhino.Close();

            using (ISession session = source.OpenSession())
            {
                try
                {
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