using System;
using CC.Core.DomainTools;
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
            sessionManager.CompleteAppointments();
            repository.Commit();
        }
    }
}