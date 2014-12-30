using System;
using System.Linq;
using CC.Core;
using CC.Core.Utilities;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;
using MF.Core.Config;
using NHibernate.Cfg;
using NHibernate.Mapping;
using NHibernate.Tool.hbm2ddl;
using ForeignKey = FluentNHibernate.Conventions.Helpers.ForeignKey;
using PrimaryKey = NHibernate.Mapping.PrimaryKey;

namespace MF.Core.Domain.Tools
{
    public class MFNHSetupConfig : INHSetupConfig
    {
        public IPersistenceConfigurer DBConfiguration(string connectionString)
        {
            return MsSqlConfiguration
                .MsSql2008
                .ConnectionString(x => x.Is(connectionString))//.Dialect<MsSqlAzureDialect>()
                .UseOuterJoin()
                .ShowSql();
        }

        public Action<MappingConfiguration> MappingConfiguration()
        {
            //m => m.AutoMappings.Add(AutoMap.AssemblyOf<User>()
            //                                    .Where(ns => ns.Namespace == "MF.Core.Domain")
            //                                        .IgnoreBase(typeof(DomainEntity))))
            return (m => m.FluentMappings.AddFromAssemblyOf<User>()
                             .Conventions.Add(ForeignKey.EndsWith("Id"))
                             .Conventions.Add(DefaultLazy.Always())
                             .Conventions.Add<ForeignKeyConstraintNameConvention>()
                             .Conventions.Add<CustomManyToManyTableNameConvention>()
                             .Conventions.Add(DefaultCascade.SaveUpdate())
                             .Conventions.Add<TextAreaConvention>());
        }

        public void GenerateSchema(Configuration config)
        {
            new SchemaExport(config).Drop(false, true);
            new SchemaExport(config).Create(true, true);
        }

        public void ClusteredIndexOnManyToMany(Configuration configuration)
        {
            configuration.CollectionMappings.Where(x => !x.IsOneToMany).ForEachItem(x =>
            {
                const string columnFormat = "{0}Id";
                var leftColumn = new Column(string.Format(
                    columnFormat,
                    x.Owner.MappedClass.Name));
                var rightColumn = new Column(string.Format(
                    columnFormat,
                    x.GenericArguments[0].Name));
                // Fetch the actual table of the many-to-many collection
                var manyToManyTable = x.CollectionTable;
                // Shorten the name just like NHibernate does
                var shortTableName = (manyToManyTable.Name.Length <= 8)
                                            ? manyToManyTable.Name
                                            : manyToManyTable.Name.Substring(0, 8);
                // Create the primary key and add the columns
                var primaryKey = new PrimaryKey
                {
                    Name = string.Format("PK_{0}", shortTableName),
                };
                primaryKey.AddColumn(leftColumn);
                primaryKey.AddColumn(rightColumn);
                // Set the primary key to the junction table
                manyToManyTable.PrimaryKey = primaryKey;
            });
        }
    }

}