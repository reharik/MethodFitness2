using System;
using MethodFitness.Core.Services;
using StructureMap;

namespace MethodFitness.SessionManagement
{
    class Program
    {
        static void Main(string[] args)
        {
            Initialize();
            var sessionManager = ObjectFactory.Container.GetInstance<ISessionManager>();
            sessionManager.CompleteAppointments();
            Console.ReadLine();
        }

        private static void Initialize()
        {
            // Bootstrapper.Restart();
//            HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();

            ObjectFactory.Initialize(x =>
            {
                x.AddRegistry(new SessionRegistry());
            });
            //ObjectFactory.AssertConfigurationIsValid();


            // HibernatingRhinos.NHibernate.Profiler.Appender.NHibernateProfiler.Initialize();


        }

    }
}
