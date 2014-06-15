using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CC.Core.Domain;
using CC.Core.Services;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using StructureMap;

namespace CC.Core.DomainTools
{
    public class Repository : IRepository
    {
        protected IUnitOfWork _unitOfWork;
        protected readonly ISystemClock _clock;

        protected Repository()
        {
        }

        public IUnitOfWork UnitOfWork
        {
            get { return _unitOfWork; }
            set { _unitOfWork = value; }
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

        public IEnumerable<T> FindAll<T>() where T : IReadableObject
        {
            return _unitOfWork.CurrentSession.Query<T>();
        }

        public void Delete<ENTITY>(ENTITY entity) where ENTITY : IPersistableObject
        {
            entity.IsDeleted = true;
            _unitOfWork.CurrentSession.SaveOrUpdate(entity);
        }

        public void HardDelete(object target)
        {
            _unitOfWork.CurrentSession.Delete(target);
        }

        public ENTITY Load<ENTITY>(int id) where ENTITY : IReadableObject
        {
            return _unitOfWork.CurrentSession.Load<ENTITY>(id);
        }

        public IQueryable<ENTITY> Query<ENTITY>() where ENTITY : IReadableObject
        {
            return _unitOfWork.CurrentSession.Query<ENTITY>();
        }

        public IQueryable<ENTITY> Query<ENTITY>(Expression<Func<ENTITY, bool>> where) 
        {
            return _unitOfWork.CurrentSession.Query<ENTITY>().Where(where);
        }

        public IEnumerable<ENTITY> ExecuteQueryOver<ENTITY>(QueryOver<ENTITY> query) where ENTITY : IReadableObject
        {
            return query.GetExecutableQueryOver(_unitOfWork.CurrentSession).List();
        }

        public IFutureValue<ENTITY> CreateQueryOverFuture<ENTITY>(QueryOver<ENTITY> query) where ENTITY : IReadableObject
        {
            return query.GetExecutableQueryOver(_unitOfWork.CurrentSession).FutureValue();
        }

        public IEnumerable<ENTITY> ExecuteSproc<ENTITY>()
        {
            return _unitOfWork.CurrentSession
                .GetNamedQuery("GetTrainerSessions")
                .SetInt32("trainerId", 1)
                .SetDateTime("appDate", DateTime.Now)
                .List<ENTITY>().ToList();
        }

        public T FindBy<T>(Expression<Func<T, bool>> where)
        {
            return _unitOfWork.CurrentSession.Query<T>().FirstOrDefault(where);
        }

        public T Find<T>(int id) where T : IReadableObject
        {
            return _unitOfWork.CurrentSession.Get<T>(id);
        }

        public IList<ENTITY> ExecuteCriteria<ENTITY>(DetachedCriteria criteria) where ENTITY : IReadableObject
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