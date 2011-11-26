using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Enumerations;
using MethodFitness.Core.Localization;
using StructureMap.Configuration.DSL.Expressions;

namespace MethodFitness.Core.Rules
{
    public interface IRule
    {
        RuleResult Execute();
    }

    public class RulesResult
    {
        public RulesResult()
        {
            Messages = new List<string>();
        }

        public bool Success { get; set; }
        public List<string> Messages { get; set; }
    }

    public class RuleResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class RuleDefinition<ENTITY> : IRule 
    {
        private readonly ENTITY _item;
        private readonly StringToken _errorMessage;

        public RuleDefinition(ENTITY item, StringToken errorMessage = null)
        {
            _item = item;
            _errorMessage = errorMessage;
            Condition = (c => true);
        }

        public Expression<Func<ENTITY, bool>> Condition { get; set; }
        public RuleResult Execute()
        {
            var ruleResult = new RuleResult{Success = true};
            bool value = Condition.Compile()(_item);
            if(!value)
            {
                ruleResult.Success = false;
                ruleResult.Message = _errorMessage!=null?_errorMessage.ToString():string.Empty;
            }
            return ruleResult;
        }
    }

    public class RuleDefinition<FIRST_ENTITY, SECOND_ENTITY> : IRule
    {
        private readonly FIRST_ENTITY _firstItem;
        private readonly SECOND_ENTITY _secondItem;
        private readonly StringToken _errorMessage;

        public RuleDefinition(FIRST_ENTITY firstItem, SECOND_ENTITY secondItem, StringToken errorMessage = null)
        {
            _firstItem = firstItem;
            _secondItem = secondItem;
            _errorMessage = errorMessage;
            Condition = ((f,s) => true);
        }

        public Expression<Func<FIRST_ENTITY, SECOND_ENTITY, bool>> Condition { get; set; }
        public RuleResult Execute()
        {
            var ruleResult = new RuleResult{Success = true};
            bool value = Condition.Compile()(_firstItem, _secondItem);
            if(!value)
            {
                ruleResult.Success = false;
                ruleResult.Message = _errorMessage != null ? _errorMessage.ToString() : string.Empty;
            }
            return ruleResult;
        }
    }
}