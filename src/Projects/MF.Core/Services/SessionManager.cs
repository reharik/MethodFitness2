﻿using System;
using System.Linq;
using CC.Core.DomainTools;
using CC.Core.ValidationServices;
using CC.Utility;
using MF.Core.Domain;
using NHibernate.Linq;

namespace MF.Core.Services
{
    public interface ISessionManager
    {
        void GatherAppointmentsDue();
        void CompleteAppointments();
    }

    public class SessionManager : ISessionManager
    {
        private readonly IRepository _repository;
        private readonly ISaveEntityService _saveEntityService;
        private readonly ILogger _logger;
        private IQueryable<Appointment> _appointments;

        public SessionManager(IRepository repository, ISaveEntityService saveEntityService, ILogger logger)
        {
            _repository = repository;
            _saveEntityService = saveEntityService;
            _logger = logger;
        }

        public void GatherAppointmentsDue()
        {
            // don't know what this is for
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
            IValidationManager validationManager = new ValidationManager(_repository);
            var appointments = _repository.Query<Appointment>(x => x.EndTime < DateTime.Now && !x.Completed).ToList();
            appointments.ToList().ForEachItem(x =>
                {
                    x.SetSessionsForClients();
                    x.Completed = true;
                    validationManager = _saveEntityService.ProcessSave(x, validationManager);
                });
            var notification = validationManager.Finish();
            var status = notification.Success?"Successful": "Failed :" +string.Join(", ", notification.Errors.Select(y=>y.ErrorMessage));
            _logger.LogInfo("Session Manger Complete Appointments: " + status);
        }
    }
}