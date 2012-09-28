using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CC.Core.Domain;
using CC.Core.DomainTools;
using CC.Core.Services;
using MethodFitness.Core.Services;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using StructureMap;

namespace MethodFitness.Core.Domain
{
    public class Repository : IRepository
    {
        private readonly IContainer _container;
        private IUnitOfWork _unitOfWork;
        private readonly ISystemClock _clock;

        public IUnitOfWork UnitOfWork
        {
            get { return _unitOfWork; }
            set { _unitOfWork = value; }
        }

        //No Filters
        public Repository()
        {
            _unitOfWork = ObjectFactory.Container.GetInstance<IUnitOfWork>("NoFilters");
            _unitOfWork.Initialize();
        }

        //No Filters or Interceptor
        public Repository(bool noFiltersOrInterceptor)
        {
            _unitOfWork = ObjectFactory.Container.GetInstance<IUnitOfWork>("NoFiltersOrInterceptor");
            _unitOfWork.Initialize();
        }
       
        public Repository(IUnitOfWork unitOfWork, ISystemClock clock)
        {
            _unitOfWork = unitOfWork;
            _unitOfWork.Initialize();
            _clock = clock;
        }

        public void DisableFilter(string FilterName)
        {
            _unitOfWork.DisableFilter(FilterName);
        }

        public void EnableFilter(string FilterName, string field, object value)
        {
            _unitOfWork.EnableFilter(FilterName, field, value);
        }

        public ISession CurrentSession()
        {
            return _unitOfWork.CurrentSession;
        }

        public void Save<ENTITY>(ENTITY entity) where ENTITY : IPersistableObject
        {
            _unitOfWork.CurrentSession.SaveOrUpdate(entity);
        }

        public IEnumerable<T> FindAll<T>() where T : IPersistableObject
        {
            return _unitOfWork.CurrentSession.Query<T>();
        }

        public void Delete<ENTITY>(ENTITY entity) where ENTITY : IPersistableObject
        {
            entity.Archived = true;
            _unitOfWork.CurrentSession.SaveOrUpdate(entity);
        }

        public void HardDelete(object target)
        {
            _unitOfWork.CurrentSession.Delete(target);
        }

        public ENTITY Load<ENTITY>(int id) where ENTITY : IPersistableObject
        {
            return _unitOfWork.CurrentSession.Load<ENTITY>(id);
        }

        public IQueryable<ENTITY> Query<ENTITY>() where ENTITY : IPersistableObject
        {
            return _unitOfWork.CurrentSession.Query<ENTITY>();
        }

        public IQueryable<ENTITY> Query<ENTITY>(Expression<Func<ENTITY, bool>> where)
        {
            return _unitOfWork.CurrentSession.Query<ENTITY>().Where(where);
        }

        public T FindBy<T>(Expression<Func<T, bool>> where)
        {
            return _unitOfWork.CurrentSession.Query<T>().FirstOrDefault(where);
        }

        public T Find<T>(int id) where T : IPersistableObject
        {
            return _unitOfWork.CurrentSession.Get<T>(id);
        }

        public IList<ENTITY> ExecuteCriteria<ENTITY>(DetachedCriteria criteria) where ENTITY : IPersistableObject
        {
            ICriteria executableCriteria = criteria.GetExecutableCriteria(_unitOfWork.CurrentSession);
            return executableCriteria.List<ENTITY>();
        }

        public IList<T> GetNamedQuery<T>(string sprocName)
        {
            return _unitOfWork.CurrentSession.GetNamedQuery(sprocName).List<T>();
        }

        public void Initialize()
        {
            _unitOfWork.Initialize();
        }

        public void Commit()
        {
            _unitOfWork.Commit();
        }

        public void Rollback()
        {
            _unitOfWork.Rollback();
        }
    }
}