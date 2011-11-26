using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MethodFitness.Core.Localization;

namespace MethodFitness.Core.Rules.ChecklistRules
{

    public class RuleBuilder<E1> : IRuleBuilder<E1>, IRuleSetBuilder<E1>, IConditionalRuleSetBuilder<E1>
    {
        private readonly BaseRulesEngine _rulesEngine;
        private int ruleSetIndex;
        private E1 _item1;

        public RuleBuilder(BaseRulesEngine rulesEngine)
        {
            _rulesEngine = rulesEngine;
        }

        public List<IRuleItem> RulesItems { get { return _rulesEngine.Rules; } }

        public IRuleSetBuilder<E1> CreateRuleSet(E1 item)
        {
            _item1 = item;
            RulesItems.Add(new RulesSet { Order = ruleSetIndex++ });
            return this;
        }

        IRuleSetBuilder<E1> IRuleSetBuilder<E1>.AddRule<RULE>(RULE instance)
        {
            var rulesItem = RulesItems.Last();
            rulesItem.Rules.Add(instance);
            return this;
        }

        IRuleSetBuilder<E1> IRuleSetBuilder<E1>.AddRule(Expression<Func<E1, bool>> expression, StringToken message = null)
        {
            var rulesItem = RulesItems.Last();
            var ruleDefinition = new RuleDefinition<E1>(_item1, message);
            ruleDefinition.Condition = expression;

            rulesItem.Rules.Add(ruleDefinition);
            return this;
        }

        public IConditionalRuleSetBuilder<E1> CreateConditionalRuleSet(E1 item)
        {
            _item1 = item;
            RulesItems.Add(new ConditionalRulesSet { Order = ruleSetIndex++ });
            return this;
        }

        IConditionalRuleSetBuilder<E1> IConditionalRuleSetBuilder<E1>.AddRule<RULE>(RULE instance)
        {
            var rulesItem = RulesItems.Last();
            rulesItem.Rules.Add(instance);
            return this;
        }

        IConditionalRuleSetBuilder<E1> IConditionalRuleSetBuilder<E1>.AddRule(Expression<Func<E1, bool>> expression, StringToken message = null)
        {
            var rulesItem = RulesItems.Last();
            var ruleDefinition = new RuleDefinition<E1>(_item1, message);
            ruleDefinition.Condition = expression;

            rulesItem.Rules.Add(ruleDefinition);
            return this;
        }

        IConditionalRuleSetBuilder<E1> IConditionalRuleSetBuilder<E1>.AddChildRule<RULE>(RULE instance)
        {
            var rulesItem = RulesItems.Last();
            ((ConditionalRulesSet)rulesItem).AddChildRule(instance);
            return this;
        }

        IConditionalRuleSetBuilder<E1> IConditionalRuleSetBuilder<E1>.AddChildRule(Expression<Func<E1, bool>> expression, StringToken message = null)
        {
            var rulesItem = RulesItems.Last();
            var ruleDefinition = new RuleDefinition<E1>(_item1, message);
            ruleDefinition.Condition = expression;
            ((ConditionalRulesSet)rulesItem).AddChildRule(ruleDefinition);
            return this;
        }

        public IRuleBuilder<E1> Next()
        {
            return this;
        }
    }

    public interface IRuleBuilder<E1>
    {
        IRuleSetBuilder<E1> CreateRuleSet(E1 item);
        IConditionalRuleSetBuilder<E1> CreateConditionalRuleSet(E1 item);
    }

    public interface IRuleSetBuilder<E1>
    {
        IRuleBuilder<E1> Next();
        IRuleSetBuilder<E1> AddRule<RULE>(RULE instance) where RULE : IRule;
        IRuleSetBuilder<E1> AddRule(Expression<Func<E1, bool>> expression, StringToken message = null);
    }

    public interface IConditionalRuleSetBuilder<E1>
    {
        IRuleBuilder<E1> Next();
        IConditionalRuleSetBuilder<E1> AddRule<RULE>(RULE instance) where RULE : IRule;
        IConditionalRuleSetBuilder<E1> AddRule(Expression<Func<E1, bool>> expression, StringToken message = null);

        IConditionalRuleSetBuilder<E1> AddChildRule<RULE>(RULE instance) where RULE : IRule;
        IConditionalRuleSetBuilder<E1> AddChildRule(Expression<Func<E1, bool>> expression, StringToken message = null);
    }
}