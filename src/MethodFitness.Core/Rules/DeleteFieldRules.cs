using System.Collections.Generic;
using MethodFitness.Core.Services;

namespace MethodFitness.Core.Rules
{
    public class DeleteFieldRules :RulesEngineBase
    {
        private readonly ISystemClock _systemClock;

        public DeleteFieldRules(ISystemClock systemClock)
        {
            _systemClock = systemClock;
            Rules = new List<IRule>();
            Rules.Add(new FieldHasNoOutstandingTasks());
            Rules.Add(new FieldHasNoOutstandingEvents(_systemClock));
        }
    }
}