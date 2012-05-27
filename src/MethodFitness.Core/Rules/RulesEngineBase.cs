using System.Collections.Generic;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Services;
using StructureMap;

namespace MethodFitness.Core.Rules
{
    public abstract class RulesEngineBase
    {
        public IValidationManager<ENTITY> ExecuteRules<ENTITY>(ENTITY entity) where ENTITY : DomainEntity
        {
            var validationManager = ObjectFactory.GetInstance<IValidationManager<ENTITY>>();
            return ExecuteRules(entity, validationManager);
        }
        public List<IRule> Rules { get; set; }
        public ValidationManager<ENTITY> ExecuteRules<ENTITY>(ENTITY entity, ValidationManager<ENTITY> validationManager) where ENTITY : DomainEntity
        {
            Rules.Each(x => validationManager.AddValidationReport(x.Execute(entity)));
            return validationManager;
        }
    }

    public interface IRule
    {
        ValidationReport Execute<ENTITY>(ENTITY entity) where ENTITY : DomainEntity;
    }

}