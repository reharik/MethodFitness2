using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CC.Core.Domain;
using NHibernate;
using NHibernate.Criterion;

namespace CC.Core.DomainTools
{
    public interface IRepository
    {
        ISession CurrentSession();

        void Save<ENTITY>(ENTITY entity)
            where ENTITY : IPersistableObject;

        ENTITY Load<ENTITY>(int id)
            where ENTITY : IReadableObject;

        IQueryable<ENTITY> Query<ENTITY>()
            where ENTITY : IReadableObject;

        IQueryable<T> Query<T>(Expression<Func<T, bool>> where);
        IEnumerable<ENTITY> ExecuteQueryOver<ENTITY>(QueryOver<ENTITY> query) where ENTITY : IReadableObject;

        T FindBy<T>(Expression<Func<T, bool>> where);

        T Find<T>(int id) where T : IReadableObject;

        IEnumerable<T> FindAll<T>() where T : IReadableObject;

        void Delete<ENTITY>(ENTITY entity) where ENTITY : IPersistableObject;

        void HardDelete(object target);


        void Commit();
        void Rollback();
        void Initialize();
        IList<ENTITY> ExecuteCriteria<ENTITY>(DetachedCriteria criteria) where ENTITY : IReadableObject;

        IList<T> GetNamedQuery<T>(string sprocName);
        void DisableFilter(string FilterName);
        void EnableFilter(string FilterName, string field, object value);
        IUnitOfWork UnitOfWork { get; set; }
        IFutureValue<ENTITY> CreateQueryOverFuture<ENTITY>(QueryOver<ENTITY> query) where ENTITY : IReadableObject;
        IEnumerable<ENTITY> ExecuteSproc<ENTITY>();
    }
}
