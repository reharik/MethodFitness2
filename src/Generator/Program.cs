using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CC.Core;
using MethodFitness.Core;
using MethodFitness.Web;
using StructureMap;

namespace Generator
{
    static class Program
    {
        static void Main(string[] args)
        {

            try
            {
                //if (args.Length >= 2 && args[1] == "production")
                //{
                //    ObjectFactory.Profile = "productionProfile";
                //}
                //ObjectFactory.Profile = "productionProfile";
                //ObjectFactory.Profile = "devProfile";

                Initialize();
                //   ObjectFactory.Profile = "productionProfile";
                //  ObjectFactory.Profile = "devProfile";
                //                var command = new EnterStringsCommand(ObjectFactory.GetInstance<ILocalizedStringLoader>(), ObjectFactory.GetInstance<IRepository>());
                //var command = new RebuildDatabaseCommand(ObjectFactory.GetInstance<ISessionSource>(), ObjectFactory.GetInstance<IRepository>(), ObjectFactory.GetInstance<ILocalizedStringLoader>(),ObjectFactory.GetInstance<PersistenceModel>());

                IGeneratorCommand command;
                var commands = ObjectFactory.GetAllInstances<IGeneratorCommand>();
                if (args.Length == 0) displayHelpAndExit(args, commands);
                command = commands.FirstOrDefault(c => c.toCanonicalCommandName() == args[0].toCanonicalCommandName());
                if (command == null) //displayHelpAndExit(args, commands);
                {
                    displayHelpAndExit(args, commands);
                    //                    command = ObjectFactory.Container.GetInstance<IGeneratorCommand>("rebuilddatabase");
                  //  command = ObjectFactory.Container.GetInstance<IGeneratorCommand>("defaultsecuritysetup");
                }
                else
                {
                    command = commands.FirstOrDefault(c => c.toCanonicalCommandName() == args[0].toCanonicalCommandName());
                }
                if (command == null) displayHelpAndExit(args, commands);
                command.Execute(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                if (Debugger.IsAttached)
                {
                    Console.ReadLine();
                }

                Environment.Exit(1);
            }
        }

        private static void displayHelpAndExit(string[] args, IEnumerable<IGeneratorCommand> commands)
        {
            if (args.Length > 0) Console.WriteLine("Unrecognized Command:" + args[0]);
            Console.WriteLine("Valid Commands are: ");

            var maxLength = commands.Max(c=>c.toCanonicalCommandName().Length);

            commands.ForEachItem(
                c =>
                Console.WriteLine("    {0, " + (maxLength + 1) + "} -> {1}", c.toCanonicalCommandName(), c.Description));

            Environment.Exit(-1);
        }

        private static void Initialize()
        {
           // Bootstrapper.Restart();
            HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();

            ObjectFactory.Initialize(x =>
                                         {
                                             x.AddRegistry(new GenRegistry());
                                             x.AddRegistry(new CommandRegistry());
                                         });
            //ObjectFactory.AssertConfigurationIsValid();


           // HibernatingRhinos.NHibernate.Profiler.Appender.NHibernateProfiler.Initialize();


        }

        public static string toCanonicalCommandName(this string commandText)
        {
            return commandText.Trim().ToLowerInvariant();
        }

        public static string toCanonicalCommandName(this IGeneratorCommand command)
        {
            return command.GetType().toCannonicalCommandName();
        }

        public static string toCannonicalCommandName(this Type commandType)
        {
            return commandType.Name.toCanonicalCommandName().Replace("command", "");
        }


    }
}
