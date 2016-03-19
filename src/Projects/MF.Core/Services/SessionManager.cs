using System;
using System.Linq;
using System.Configuration;
using CC.Core.Core.DomainTools;
using CC.Core.Core.ValidationServices;
using CC.Core.Utilities;
using MF.Core.Domain;
using System.Data.SqlClient;
using System.Data;

namespace MF.Core.Services
{
    public interface ISessionManager
    {
//        void GatherAppointmentsDue();
        void CompleteAppointments();
    }

    public class SessionManager : ISessionManager
    {
        private readonly IRepository _repository;
        private readonly ISaveEntityService _saveEntityService;
        private readonly ILogger _logger;
        private readonly IClientSessionService _clientSessionService;
        private IQueryable<Appointment> _appointments;

        public SessionManager(IRepository repository, ISaveEntityService saveEntityService, ILogger logger,
                              IClientSessionService clientSessionService)
        {
            _repository = repository;
            _saveEntityService = saveEntityService;
            _logger = logger;
            _clientSessionService = clientSessionService;

        }

        public void CompleteAppointments()
        {
            _logger.LogInfo("Beginning CompleteAppointments Process.");

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.AppSettings["MethodFitness.sql_server_connection_string"])){
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = "SessionReconciliation";
                command.CommandType = CommandType.StoredProcedure;
                
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
        }


        //    _repository.CreateSQLQuery<DomainEntity>("exec SessionReconciliation", null, 30);
            //IValidationManager validationManager = new ValidationManager(_repository);
          
            //var appointment = _repository.Query<Appointment>(x => x.EndTime < DateTime.Now && !x.Completed).Take(1).FirstOrDefault();
            //while (appointment != null)
            //{
            //    _logger.LogInfo("Session Mananger Processing aptId:{0}".ToFormat(appointment.EntityId));
            //    _clientSessionService.SetSessionsForClients(appointment);
            //    appointment.Completed = true;
            //    appointment.Clients.Where(c => c.ClientStatus != null && c.ClientStatus.AdminAlerted).ForEachItem(c => c.ClientStatus.AdminAlerted = false);
            //    validationManager = _saveEntityService.ProcessSave(appointment, validationManager);
            //    _logger.LogDebug("about to save appointment: {0}".ToFormat(appointment.EntityId));
            //    validationManager.Finish();
            //    appointment = _repository.Query<Appointment>(x => x.EndTime < DateTime.Now && !x.Completed).Take(1).FirstOrDefault();
            //}
        }
    }
}

