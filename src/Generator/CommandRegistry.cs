using FluentNHibernate.Conventions;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Services;
using StructureMap.Configuration.DSL;

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