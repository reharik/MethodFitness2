using System.Collections.Generic;
using CC.Core.DomainTools;
using CC.Core.Services;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Services;

namespace MethodFitness.Core.Rules
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