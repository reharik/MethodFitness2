using System;
using CC.Core.Core.DomainTools;
using MF.Core;
using MF.Core.Services;
using Quartz;
using StructureMap;

namespace MF.SessionManagement
{
    public class ClientSessionJob:IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            var sessionManager = ObjectFactory.Container.GetInstance<ISessionManager>();
            var repository = ObjectFactory.Container.GetInstance<IRepository>();
            var logger = ObjectFactory.Container.GetInstance<ILogger>();
            sessionManager.CompleteAppointments();
            // ok I have nfi what's going on here I found commit commented out and rollback live.
            // I also found code in the completeappointments class that calles validation manager.finish
            // commented out.  I'm going to un comment that shit and see what happens.
            //            repository.Commit();
//            repository.Rollback();
            logger.LogDebug("Session Manager fired at: "+DateTime.Now.ToString());
        }
    }
}