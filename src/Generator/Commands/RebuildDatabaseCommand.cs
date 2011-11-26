using MethodFitness.Core.Domain;
using NHibernate;
using NHibernate.Cfg;
using StructureMap;
using StructureMap.Pipeline;
using ISessionFactoryConfiguration = MethodFitness.Core.Domain.ISessionFactoryConfiguration;

//using ISessionFactoryConfiguration = MethodFitness.Core.Domain.ISessionFactoryConfiguration;

namespace Generator.Commands
{
    public class RebuildDatabaseCommand : IGeneratorCommand
    {
        private readonly ILocalizedStringLoader _loader;
        private  IRepository _repository;

        public RebuildDatabaseCommand(ILocalizedStringLoader loader)
        {
            _loader = loader;
        }

        public string Description { get { return "Rebuilds the db and data"; } }

        public void Execute(string[] args)
        {
            SqlServerHelper.KillAllFKs();

            ObjectFactory.Configure(x => x.For<ISessionFactory>().Singleton().Use(ctx => ctx.GetInstance<ISessionFactoryConfiguration>().CreateSessionFactoryAndGenerateSchema()));
            
            var sessionFactory = ObjectFactory.GetInstance<ISessionFactory>();

            SqlServerHelper.GenerateSecurityTables(sessionFactory);
            _repository = ObjectFactory.Container.GetInstance<Repository>("NoFiltersOrInterceptor");
            new DataLoader().Load(null);
        }
    }
}