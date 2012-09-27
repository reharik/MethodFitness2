using CC.Core.DomainTools;
using MethodFitness.Core.Domain;
using NHibernate;
using StructureMap;


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
//            ObjectFactory.Configure(x => x.For<ISessionFactory>().Singleton().Use(ctx => ctx.GetInstance<ISessionFactoryConfiguration>().CreateSessionFactory()));
            var sessionFactory = ObjectFactory.GetInstance<ISessionFactory>();
//            SqlServerHelper.DeleteReaddDb(sessionFactory);

            ObjectFactory.Configure(x => x.For<ISessionFactory>().Singleton().Use(ctx => ctx.GetInstance<ISessionFactoryConfiguration>().CreateSessionFactoryAndGenerateSchema()));
            sessionFactory = ObjectFactory.GetInstance<ISessionFactory>();

            new DataLoader().Load();
            SqlServerHelper.AddRhinoSecurity(sessionFactory);

            ObjectFactory.ResetDefaults();
            ObjectFactory.Initialize(x =>
            {
                x.AddRegistry(new GenRegistry());
                x.AddRegistry(new CommandRegistry());
            });
            var securitySetup = ObjectFactory.Container.GetInstance<IGeneratorCommand>("defaultsecuritysetup");
            securitySetup.Execute(null);














//
//            new DataLoader().Load();
//            SqlServerHelper.AddRhinoSecurity(sessionFactory);
//
//            var securitySetup = ObjectFactory.Container.GetInstance<IGeneratorCommand>("defaultsecuritysetup");
//            securitySetup.Execute(null);


            //_loader.ClearStrings();
            //_loader.LoadStrings(_repository);
        }
    }
}