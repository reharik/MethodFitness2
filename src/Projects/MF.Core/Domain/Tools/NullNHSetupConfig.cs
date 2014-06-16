using System;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using MF.Core.Config;
using NHibernate.Cfg;

namespace MF.Core.Domain.Tools
{
    public class NullNHSetupConfig : INHSetupConfig
    {
        public IPersistenceConfigurer DBConfiguration(string connectionString)
        {
            return null;
        }

        public Action<MappingConfiguration> MappingConfiguration()
        {
            return null;
        }

        public void GenerateSchema(Configuration config)
        {
            return;
        }

        public void ClusteredIndexOnManyToMany(Configuration configuration)
        {
            throw new NotImplementedException();
        }
    }

}