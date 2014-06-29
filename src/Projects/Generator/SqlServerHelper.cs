using System;
using System.Data;
using System.IO;
using MF.Core.Config;
using NHibernate;
using StructureMap;

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
            var connstring = "";
            using (ISession session = source.OpenSession())
            {
                var sql =
                    "USE [master] alter database MethodFitness_DEV set single_user with rollback immediate DROP DATABASE MethodFitness_DEV CREATE DATABASE MethodFitness_DEV";
                connstring = session.Connection.ConnectionString;
                IDbConnection conn = session.Connection;
                var cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }

            ObjectFactory.Container.Dispose();
            ObjectFactory.Initialize(x =>
            {
                x.AddRegistry(new GenRegistry());
                x.AddRegistry(new CommandRegistry());
            });

            var sessionFactoryConfiguration =
                 ObjectFactory.Container.With("connectionStr")
                              .EqualTo(connstring)
                              .GetInstance<ISessionFactoryConfiguration>();
            ObjectFactory.Container.Inject(sessionFactoryConfiguration);

        }

        public static void killRhinoSecurity(ISessionFactory source)
        {
            using (ISession session = source.OpenSession(new SaveUpdateInterceptor()))
            {
                var sql =
                    "delete security_UsersToUsersGroups;delete security_UsersGroupsHierarchy;delete security_Permissions;delete security_UsersGroups;delete security_Operations;";

                IDbConnection conn = session.Connection;
                var cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }
        }

        public static void AddRhinoSecurity(ISessionFactory source)
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