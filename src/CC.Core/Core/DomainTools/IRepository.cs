using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CC.Core.Core.Domain;
using CC.Core.Security;
using NHibernate;
using NHibernate.Criterion;

namespace CC.Core.Core.DomainTools
{
    public interface IRepository
    {
        ISession CurrentSession();

        void Save<ENTITY>(ENTITY entity)
            where ENTITY : Entity;

        ENTITY Load<ENTITY>(int id)
            where ENTITY : Entity;

        IQueryable<ENTITY> Query<ENTITY>();

        IQueryable<T> Query<T>(Expression<Func<T, bool>> where);
        IEnumerable<ENTITY> ExecuteQueryOver<ENTITY>(QueryOver<ENTITY> query) where ENTITY : Entity;

        T FindBy<T>(Expression<Func<T, bool>> where);

        T Find<T>(int id) where T : Entity;

        IEnumerable<T> FindAll<T>() where T : Entity;

        void Delete<ENTITY>(ENTITY entity) where ENTITY : Entity;

        void HardDelete(object target);
        IList<T> CreateSQLQuery<T>(string sql, object properties) where T : class;
        IList<T> CreateSQLQuery<T>(string sql, object properties, int setTimeoutSeconds) where T : class;
        void AssociateUserWith(IUser user, string groupName);
        void DetachUserFromGroup(IUser user, string usersGroupName);
        void Commit();
        void Rollback();
        void Initialize();
        IList<ENTITY> ExecuteCriteria<ENTITY>(DetachedCriteria criteria) where ENTITY : Entity;

        IList<T> GetNamedQuery<T>(string sprocName);
        void DisableFilter(string FilterName);
        void EnableFilter(string FilterName, string field, object value);
        IUnitOfWork UnitOfWork { get; set; }
        IFutureValue<ENTITY> CreateQueryOverFuture<ENTITY>(QueryOver<ENTITY> query) where ENTITY : Entity;
        IEnumerable<ENTITY> ExecuteSproc<ENTITY>();
    }
}
