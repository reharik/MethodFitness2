using CC.Core.Domain;
using CC.Core.DomainTools;

namespace CC.Core.ValidationServices
{
    public interface ISaveEntityService
    {
        IValidationManager ProcessSave<DOMAINMODEL>(DOMAINMODEL model, IValidationManager validationManager) where DOMAINMODEL : IPersistableObject;
        IValidationManager ProcessSave<DOMAINMODEL>(DOMAINMODEL model) where DOMAINMODEL : IPersistableObject;
    }

    public class SaveEntityService : ISaveEntityService
    {
        private readonly IRepository _repository;
        private readonly ICCValidationRunner _validationRunner;

        public SaveEntityService(IRepository repository, ICCValidationRunner validationRunner)
        {
            _repository = repository;
            _validationRunner = validationRunner;
        }

        public IValidationManager ProcessSave<DOMAINMODEL>(DOMAINMODEL model) where DOMAINMODEL : IPersistableObject
        {
            var crudManager = new ValidationManager(_repository);
            return ProcessSave(model, crudManager);
        }

        public IValidationManager ProcessSave<DOMAINMODEL>(DOMAINMODEL model, IValidationManager validationManager)
            where DOMAINMODEL : IPersistableObject
        {
            var entity = model as Entity;
            var report = _validationRunner.Validate(entity);
            if (report.Success)
            {
                _repository.Save(model);
                //report.Target = model;
            }
            validationManager.AddValidationReport(report);
            return validationManager;
        }
    }

    // you can't turn off the interceptor you have to have a session object with never recieved one.
    // repo is request scoped so once you get one your stuck with it.  the orgional ses has repo 
    // constructor injected so you screwed must use one that doesn't have it in the constructor
    public interface ISaveEntityServiceWithoutPrincipal
    {
        IValidationManager ProcessSave<DOMAINMODEL>(DOMAINMODEL model) where DOMAINMODEL : IPersistableObject;
    }

    public class NullSaveEntityServiceWithoutPrincipal : ISaveEntityServiceWithoutPrincipal
    {
        private readonly ICCValidationRunner _validationRunner;

        public NullSaveEntityServiceWithoutPrincipal(ICCValidationRunner validationRunner)
        {
            _validationRunner = validationRunner;
        }

        public IValidationManager ProcessSave<DOMAINMODEL>(DOMAINMODEL model) where DOMAINMODEL : IPersistableObject
        {
            throw new System.NotImplementedException();
        }
    }

    //    public class SaveEntityServiceWithoutPrincipal : ISaveEntityServiceWithoutPrincipal
    //    {
    //        private readonly ICastleValidationRunner _castleValidationRunner;
    //        private IRepository _repository;
    //
    //        //public SaveEntityServiceWithoutPrincipal(IContainer container, ICastleValidationRunner validationRunner)
    //        //{
    //        //    _castleValidationRunner = validationRunner;
    //        //    _repository = container.GetInstance<Repository>("NoFiltersOrInterceptor");
    //        //}
    //
    //        public IValidationManager ProcessSave<DOMAINMODEL>(DOMAINMODEL model) where DOMAINMODEL : IPersistableObject
    //        {
    //            //var report = _castleValidationRunner.Validate(model);
    //            //if (report.Success)
    //            //{
    //            //    _repository.Save(model);
    //            //    //report.Target = model;
    //            //}
    //            //var ValidationManager = new ValidationManager(_repository);
    //            //ValidationManager.AddValidationReport(report);
    //            //return ValidationManager;
    //            return null;
    //        }
    //    }
}