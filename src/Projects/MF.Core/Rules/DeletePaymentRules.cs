using System.Collections.Generic;
using CC.Core.Core.DomainTools;
using CC.Core.Core.Services;

namespace MF.Core.Rules
{
    public class DeletePaymentRules :RulesEngineBase
    {
        private readonly ISystemClock _systemClock;
        private readonly IRepository _repository;

        public DeletePaymentRules(ISystemClock systemClock,IRepository repository)
        {
            _systemClock = systemClock;
            _repository = repository;
            Rules = new List<IRule>();
            Rules.Add(new NoTrainerHasBeenPaidForPaymentSessions(_systemClock,_repository));
        }
    }
}