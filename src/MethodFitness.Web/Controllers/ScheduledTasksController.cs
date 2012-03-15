using System;
using System.Web.Mvc;
using MethodFitness.Core;
using MethodFitness.Core.Domain;
using MethodFitness.Web.Areas.Schedule.Controllers;
using StructureMap;

namespace MethodFitness.Web.Controllers
{
    public class ScheduledTasksController : Controller
    {
        private IRepository _repository;

        public ScheduledTasksController()
        {
            _repository = ObjectFactory.Container.GetInstance<IRepository>("NoFiltersOrInterceptor");
        }

        public ActionResult ProcessAppointments()
        {
            var appointments = _repository.Query<Appointment>(x => x.EndTime > DateTime.Now.AddDays(-1) && x.EndTime <= DateTime.Now);
            appointments.Each(x =>
                                  {
                                      x.Sessions.Each(s =>
                                                          {
                                                              s.SessionUsed = true;
                                                              s.Trainer = x.Trainer;
                                                              x.Trainer.AddSession(s);
                                                          });
                                      _repository.Save(x);
                                  });
            return null;
        }
    }
}