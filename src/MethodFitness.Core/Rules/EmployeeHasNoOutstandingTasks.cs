using System.Linq;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Services;
using MethodFitness.Web.Areas.Schedule.Controllers;
using xVal.ServerSide;

namespace MethodFitness.Core.Rules
{
    public class ClientHasNoOutstandingAppointments : IRule
    {
        private readonly ISystemClock _systemClock;
        private readonly IRepository _repository;

        public ClientHasNoOutstandingAppointments(ISystemClock systemClock, IRepository repository)
        {
            _systemClock = systemClock;
            _repository = repository;
        }

        public ValidationReport Execute<ENTITY>(ENTITY client) where ENTITY : DomainEntity
        {
            var result = new ValidationReport {Success = true};
            var _client = client as Client;
            var appointments = _repository.Query<Appointment>(x => x.Clients.Any(i => i == _client));
            if (appointments.Any())
            {
                result.Success = false;
                result.AddErrorInfo(new ErrorInfo("Rule",CoreLocalizationKeys.CLIENT_HAS_APPOINTMENTS_IN_FUTURE.ToFormat(appointments.Count())));
            }
            return result;
        }
    }
}