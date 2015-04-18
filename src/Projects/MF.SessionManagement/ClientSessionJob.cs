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
//            repository.Commit();
            repository.Rollback();
            logger.LogDebug("Session Manager fired at: "+DateTime.Now.ToString());
        }
    }
}