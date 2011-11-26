using System.Collections.Generic;
using System.Linq;
using MethodFitness.Core.Enumerations;
using MethodFitness.Core.Rules.ChecklistRules;

namespace MethodFitness.Core.Rules
{
    public abstract class BaseRulesEngine
    {

        protected BaseRulesEngine()
        {
            Rules = new List<IRuleItem>();
        }

        protected internal List<IRuleItem> Rules { get; set; }
        public virtual RulesResult ExecuteRules()
        {
            RulesResult rulesResult = new RulesResult {Success = true};
            foreach (var rule in Rules.OrderBy(x=>x.Order))
            {
                if (rule.GetType().GetInterfaces().Any(x=>x.Name == "IRulesSet"))
                {
                    ((IRulesSet)rule).ProcessRulesSet(rulesResult);
                }
                if (rule.GetType() == typeof(RuleItemOperator))
                {
                   
                }
            }
            return rulesResult;
        }

        
    }

    public class ConditionalRulesSet : IRuleItem, IRulesSet
    {
        public ConditionalRulesSet()
        {
            Rules = new List<IRule>();
            ChildRules = new RulesSet();
        }
        public int Order { get; set; }
        public List<IRule> Rules { get; set; }
        public RulesSet ChildRules { get; set; }
        public void AddChildRule(IRule rule)
        {
            ChildRules.Rules.Add(rule);
        }

        public RulesResult ProcessRulesSet(RulesResult rulesResult)
        {
            var localRulesResult = new RulesResult {Success = true};
            Rules.Each(x =>
            {
                var ruleResult = x.Execute();
                if (!ruleResult.Success)
                {
                    localRulesResult.Success = false;
                    localRulesResult.Messages.Add(ruleResult.Message);
                }
            });
            if (localRulesResult.Success)
            {
                ChildRules.ProcessRulesSet(rulesResult);
            }
            return rulesResult;
        }
    }


    public class RulesSet : IRuleItem, IRulesSet
    {
        public RulesSet()
        {
            Rules = new List<IRule>();
        }
        public int Order { get; set; }
        public List<IRule> Rules { get; set; }
        public RulesResult ProcessRulesSet(RulesResult rulesResult)
        {
            Rules.Each(x =>
            {
                var ruleResult = x.Execute();
                if (!ruleResult.Success)
                {
                    rulesResult.Success = false;
                    rulesResult.Messages.Add(ruleResult.Message);
                }
            });
            return rulesResult;
        }
    }

    public interface IRulesSet
    {
        RulesResult ProcessRulesSet(RulesResult rulesResult);
        List<IRule> Rules { get; set; }
    }

    public class RuleItemOperator : IRuleItem
    {
        public List<IRule> Rules { get; set; }
        public RuleOperatorEnum OperatorEnum { get; set; }
        public bool ContinueIfLeftIsFalse { get; set; }
        public int Order { get; set; }
    }

    public interface IRuleItem
    {
        int Order { get; set; }
        List<IRule> Rules { get; set; }
    }
}