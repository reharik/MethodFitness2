using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CC.Core.Core.Domain;
using CC.Core.Core.Services;
using CC.Core.Security.Impl;
using CC.Core.Security.Model;
using CC.Core.Security;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.Transform;

namespace CC.Core.Core.DomainTools
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

        public void Save<ENTITY>(ENTITY entity) where ENTITY : Entity
        {
            _unitOfWork.CurrentSession.SaveOrUpdate(entity);
        }

        public IEnumerable<T> FindAll<T>() where T : Entity
        {
            return _unitOfWork.CurrentSession.Query<T>();
        }

        public void Delete<ENTITY>(ENTITY entity) where ENTITY : Entity
        {
            entity.IsDeleted = true;
            _unitOfWork.CurrentSession.SaveOrUpdate(entity);
        }

        public void HardDelete(object target)
        {
            _unitOfWork.CurrentSession.Delete(target);
        }

        public IList<T> CreateSQLQuery<T>(string sql, object properties) where T : class
        {
            return CreateSQLQuery<T>(sql, properties, 30);
        }

        public IList<T> CreateSQLQuery<T>(string sql, object properties, int setTimeoutSeconds) where T : class
        {
            // stored proc syntax is: "exec proc_name :param1, :param2"
            var query = _unitOfWork.CurrentSession.CreateSQLQuery(sql)
                .SetProperties(properties)
                .SetTimeout(setTimeoutSeconds)
                .SetResultTransformer(Transformers.AliasToBean<T>())
                .List<T>();
            return query;
        }

        public ENTITY Load<ENTITY>(int id) where ENTITY : Entity
        {
            return _unitOfWork.CurrentSession.Load<ENTITY>(id);
        }

        public IQueryable<ENTITY> Query<ENTITY>() 
        {
            return _unitOfWork.CurrentSession.Query<ENTITY>();
        }

        public IQueryable<ENTITY> Query<ENTITY>(Expression<Func<ENTITY, bool>> where) 
        {
            return _unitOfWork.CurrentSession.Query<ENTITY>().Where(where);
        }

        public IEnumerable<ENTITY> ExecuteQueryOver<ENTITY>(QueryOver<ENTITY> query) where ENTITY : Entity
        {
            return query.GetExecutableQueryOver(_unitOfWork.CurrentSession).List();
        }

        public IFutureValue<ENTITY> CreateQueryOverFuture<ENTITY>(QueryOver<ENTITY> query) where ENTITY : Entity
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

        public T Find<T>(int id) where T : Entity
        {
            return _unitOfWork.CurrentSession.Get<T>(id);
        }

        public IList<ENTITY> ExecuteCriteria<ENTITY>(DetachedCriteria criteria) where ENTITY : Entity
        {
            ICriteria executableCriteria = criteria.GetExecutableCriteria(_unitOfWork.CurrentSession);
            return executableCriteria.List<ENTITY>();
        }

        public IList<T> GetNamedQuery<T>(string sprocName)
        {
            return _unitOfWork.CurrentSession.GetNamedQuery(sprocName).List<T>();
        }
        public void AssociateUserWith(IUser user, string groupName)
        {
            UsersGroup group = _unitOfWork.CurrentSession.CreateCriteria<UsersGroup>()
                .Add(Restrictions.Eq("Name", groupName))
                .UniqueResult<UsersGroup>();
            Guard.Against(group == null, "There is no users group named: " + groupName);
            group.Users.Add(user);
            _unitOfWork.CurrentSession.SaveOrUpdate(group);
        }
        public void DetachUserFromGroup(IUser user, string usersGroupName)
        {
            UsersGroup group = _unitOfWork.CurrentSession.CreateCriteria<UsersGroup>()
               .Add(Restrictions.Eq("Name", usersGroupName))
               .UniqueResult<UsersGroup>();
            Guard.Against(group == null, "There is no users group named: " + usersGroupName);

            group.Users.Remove(user);
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