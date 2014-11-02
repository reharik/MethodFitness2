using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CC.Core.DomainTools;
using MF.Core.Services;
using StructureMap;

namespace MF.ClientDropOffReport
{
    class Program
    {
        private static void Main(string[] args)
        {
            Initialize();
            var service = ObjectFactory.Container.GetInstance<IGetDroppedClients>();
            var droppedClients = service.GetClients();
            if (!droppedClients.Any())
            {
                Console.WriteLine("No new dropped clients");
                return;
            }
            var email = service.CreateEmail(droppedClients);
            service.SendEmail(email);
            service.UpdateClients(droppedClients);
        }

        private static void Initialize()
        {
            // Bootstrapper.Restart();
            //            HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();

            ObjectFactory.Initialize(x =>
            {
                x.AddRegistry(new ClientDropOffRegistry());
            });
            //ObjectFactory.AssertConfigurationIsValid();


            // HibernatingRhinos.NHibernate.Profiler.Appender.NHibernateProfiler.Initialize();


        }
    }
}
