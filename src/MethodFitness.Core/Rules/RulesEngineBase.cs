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
            var repository = ObjectFactory.GetInstance<IRepository>();
            var validationManager = new ValidationManager<ENTITY>(repository);
            return ExecuteRules(entity, validationManager);
        }
        public List<IRule> Rules { get; set; }
        public IValidationManager<ENTITY> ExecuteRules<ENTITY>(ENTITY entity, IValidationManager<ENTITY> validationManager) where ENTITY : DomainEntity
        {
            Rules.ForEachItem(x => validationManager.AddValidationReport(x.Execute(entity)));
            return validationManager;
        }
    }

    public interface IRule
    {
        ValidationReport<ENTITY> Execute<ENTITY>(ENTITY entity) where ENTITY : DomainEntity;
    }

}