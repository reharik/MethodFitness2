using System.Collections.Generic;
using CC.Core;
using CC.Core.DomainTools;
using CC.Core.Services;
using StructureMap;

namespace MethodFitness.Core.Rules
{
    public abstract class RulesEngineBase
    {
        public IValidationManager ExecuteRules<ENTITY>(ENTITY entity) where ENTITY : class
        {
            var repository = ObjectFactory.GetInstance<IRepository>();
            var validationManager = new ValidationManager(repository);
            return ExecuteRules(entity, validationManager);
        }
        public List<IRule> Rules { get; set; }
        public IValidationManager ExecuteRules<ENTITY>(ENTITY entity, IValidationManager validationManager) where ENTITY : class
        {
            Rules.ForEachItem(x => validationManager.AddValidationReport(x.Execute(entity)));
            return validationManager;
        }
    }

    public interface IRule
    {
        ValidationReport Execute<ENTITY>(ENTITY entity) where ENTITY : class;
    }

}