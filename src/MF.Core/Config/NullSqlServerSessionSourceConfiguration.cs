using NHibernate;

namespace MF.Core.Config
{
    public class NullSqlServerSessionSourceConfiguration : ISessionFactoryConfiguration
    {
        private readonly INHSetupConfig _config;
        private readonly string _connectionStr;

        public NullSqlServerSessionSourceConfiguration(INHSetupConfig config)
        {
            _config = config;
        }

        public ISessionFactory CreateSessionFactoryAndGenerateSchema()
        {
            return null;
        }

        public ISessionFactory CreateSessionFactory()
        {
            return null;
        }

    } 
}