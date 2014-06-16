using System;
using System.Web.Mvc;
using CC.Core;
using CC.Core.DomainTools;
using MF.Core.Domain;
using StructureMap;

namespace MF.Web.Controllers
{
    public class ScheduledTasksController : Controller
    {
        private IRepository _repository;

        public ScheduledTasksController()
        {
            _repository = ObjectFactory.Container.GetInstance<IRepository>("NoInterceptorNoFiltersUnitOfWork");
        }

        public ActionResult ProcessAppointments()
        {
            var appointments = _repository.Query<Appointment>(x => x.EndTime > DateTime.Now.AddDays(-1) && x.EndTime <= DateTime.Now);
            appointments.ForEachItem(x =>
                                  {
                                      x.Sessions.ForEachItem(s =>
                                                          {
                                                              s.SessionUsed = true;
                                                              s.Trainer = x.Trainer;
                                                              x.Trainer.AddAppointment(x);
                                                          });
                                      _repository.Save(x);
                                  });
            return null;
        }
    }
}