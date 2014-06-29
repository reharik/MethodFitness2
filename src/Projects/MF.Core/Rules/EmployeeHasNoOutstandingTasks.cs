using System.Linq;
using CC.Core.Domain;
using CC.Core.DomainTools;
using CC.Core.Services;
using CC.Core.ValidationServices;
using CC.DataValidation;
using MF.Core.Domain;

namespace MF.Core.Rules
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

        public ValidationReport Execute<ENTITY>(ENTITY client) where ENTITY : Entity
        {
            var _client = client as Client;
            var result = new ValidationReport{ Success = true, entity = _client};
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