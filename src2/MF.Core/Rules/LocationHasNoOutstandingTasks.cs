using System.Linq;
using CC.Core.Core.Domain;
using CC.Core.Core.DomainTools;
using CC.Core.Core.Services;
using CC.Core.Core.ValidationServices;
using CC.Core.DataValidation;
using MF.Core.Domain;

namespace MF.Core.Rules
{
    public class LocationHasNoOutstandingAppointments : IRule
    {
        private readonly ISystemClock _systemClock;
        private readonly IRepository _repository;

        public LocationHasNoOutstandingAppointments(ISystemClock systemClock, IRepository repository)
        {
            _systemClock = systemClock;
            _repository = repository;
        }

        public ValidationReport Execute<ENTITY>(ENTITY location) where ENTITY : Entity
        {
            var result = new ValidationReport { Success = true,entity = location};
            var _location = location as Location;
            var appointments = _repository.Query<Appointment>(x => x.Location == _location);
            if (appointments.Any())
            {
                result.Success = false;
                result.AddErrorInfo(new ErrorInfo("Rule", CoreLocalizationKeys.LOCATION_HAS_APPOINTMENTS_IN_FUTURE.ToFormat(appointments.Count())));
            }
            return result;
        }
    }
}