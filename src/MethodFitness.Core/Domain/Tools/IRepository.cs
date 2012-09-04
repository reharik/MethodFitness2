using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate;
using NHibernate.Criterion;

namespace MethodFitness.Core.Domain
{
    public interface IRepository
    {
        ISession CurrentSession();

        void Save<ENTITY>(ENTITY entity)
            where ENTITY : IPersistableObject;

        ENTITY Load<ENTITY>(int id)
            where ENTITY : IPersistableObject;

        IQueryable<ENTITY> Query<ENTITY>()
            where ENTITY : IPersistableObject;

        IQueryable<T> Query<T>(Expression<Func<T, bool>> where);

        T FindBy<T>(Expression<Func<T, bool>> where);

        T Find<T>(int id) where T : IPersistableObject;

        IEnumerable<T> FindAll<T>() where T : IPersistableObject;

        void Delete<ENTITY>(ENTITY entity) where ENTITY : IPersistableObject;

        void HardDelete(object target);


        void Commit();
        void Rollback();
        void Initialize();
        IList<ENTITY> ExecuteCriteria<ENTITY>(DetachedCriteria criteria) where ENTITY : IPersistableObject;

        IList<T> GetNamedQuery<T>(string sprocName);
        void DisableFilter(string FilterName);
        void EnableFilter(string FilterName, string field, object value);
        IUnitOfWork UnitOfWork { get; set; }
    }
}
