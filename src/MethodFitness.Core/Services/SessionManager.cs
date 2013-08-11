using System;
using System.Linq;
using CC.Core;
using CC.Core.DomainTools;
using MethodFitness.Core.Domain;
using NHibernate.Linq;

namespace MethodFitness.Core.Services
{
    public class SessionManager
    {
        private readonly IRepository _repository;
        private IQueryable<Appointment> _appointments;

        public SessionManager(IRepository repository)
        {
            _repository = repository;
        }

        public void GatherAppointmentsDue()
        {
            // I don't know why this is a field.
            _appointments = _repository.Query<Appointment>(x=>x.EndTime < DateTime.Now && x.Completed)
                .FetchMany(x=>x.Clients).ThenFetch(x=>x.Sessions);
            _appointments.ForEachItem(x =>
                {
//                    do somthing
                });
        }

        public void CompleteAppointments()
        {
            var appointments = _repository.Query<Appointment>(x => x.EndTime < DateTime.Now && !x.Completed)
                .FetchMany(x => x.Clients).ThenFetch(x => x.Sessions);
            appointments.ForEachItem(x =>
                {
                    x.SetSessionsForClients();

                });
        } 
    }
}