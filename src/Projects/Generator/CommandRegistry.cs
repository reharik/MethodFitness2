using FluentNHibernate.Conventions;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;

namespace Generator
{
    public class CommandRegistry : Registry
    {
        public CommandRegistry()
        {
            Scan(x =>
                     {
                         x.TheCallingAssembly();
                         x.AddAllTypesOf<IGeneratorCommand>().NameBy(t => t.toCannonicalCommandName());
                     });
           
            For<IConventionFinder>().Use<DefaultConventionFinder>();
            For<ILocalizedStringLoader>().Use<LocalizedStringLoader>();

        }
    }
}