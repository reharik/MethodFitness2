using System;
using System.Collections.Generic;
using NHibernate;

namespace CC.Core.Core.DomainTools
{
    public class UnitOfWork : IUnitOfWork
    {
        protected ITransaction Transaction;
        private bool _isDisposed;
        protected ISessionFactory SessionFactory;
        private bool _isInitialized;

        protected readonly List<Tuple<string, string, object>> EnabledFilterList;
        protected readonly List<string> DisabledFilterList;

        public ISession CurrentSession { get; private set; }
        public IStatelessSession CurrentStatelessSession { get; private set; }
        protected UnitOfWork()
        {
        }
        public UnitOfWork(ISessionFactory sessionFactory)
        {
            SessionFactory = sessionFactory;
             
            EnabledFilterList = new List<Tuple<string, string, object>>();
            DisabledFilterList = new List<string>();
        }

        public void DisableFilter(string filterName)
        {
            // store it in the DisableFilters collection
            if (!DisabledFilterList.Contains(filterName))
                DisabledFilterList.Add(filterName);

            // then disable it
            CurrentSession.DisableFilter(filterName);
        }

        public void EnableFilter(string filterName, string field, object value)
        {
            // store it in EnabledFilters collection
            var tuple = new Tuple<string, string, object>(filterName, field, value);
            if (!EnabledFilterList.Contains(tuple))
                EnabledFilterList.Add(tuple);

            // then apply it
            ApplyEnableFilter(filterName, field, value);
        }

        protected void ApplyEnableFilter(string filterName, string field, object value)
        {
            var enableFilter = CurrentSession.EnableFilter(filterName);
            enableFilter.SetParameter(field, value);
        }

        protected virtual void ReestablishFilters()
        {
            // loop through list of filters and reestablish them on the (likely recently replaced) CurrentSession
            EnabledFilterList.ForEach(tuple => ApplyEnableFilter(tuple.Item1, tuple.Item2, tuple.Item3));
            DisabledFilterList.ForEach(CurrentSession.DisableFilter);
        }

        public bool IsInitialized()
        {
            return _isInitialized;
        }

        public void Initialize()
        {
            should_not_currently_be_disposed();
            if (_isInitialized) return;

            CreateNewCurrentSession();

            _isInitialized = true;
        }

        public void Commit()
        {
            should_not_currently_be_disposed();
            should_be_initialized_first();

            // create a transaction for all pending changes in CurrentSession
            begin_new_transaction();

            try
            {
                Transaction.Commit();
            }
            catch (Exception)
            {
                Transaction.Rollback();
                throw;
            }
        }

        /// <summary>
        /// Clear all pending changes for the UnitOfWork
        /// </summary>
        /// <remarks>
        /// NOTE: Any deferred IQueryables or IEnumerables established with the previous session will blow up!
        /// It is safe, however, to get new Queries from the same Repository that previously got the now-defunct Queries.
        /// </remarks>
        public void Rollback()
        {
            should_not_currently_be_disposed();
            should_be_initialized_first();

            // no need to rollback anything, nothing's been pushed to the DB yet
            // just create a new CurrentSession
            CreateNewCurrentSession();
        }

        protected void CreateNewCurrentSession()
        {
            if (CurrentSession != null)
            {
                CurrentSession.Clear();
                CurrentSession.Close();
            }

            if (CurrentStatelessSession != null)
            {
                CurrentStatelessSession.Close();
            }

            var session = SessionFactory.OpenSession();
            // should probably make this value somehow injected or modifyable
            session.FlushMode = FlushMode.Commit;
            CurrentSession = session;
            CurrentStatelessSession = SessionFactory.OpenStatelessSession();

            // reestablish the filters on this new session
            ReestablishFilters();
        }

        private void begin_new_transaction()
        {
            if (Transaction != null)
            {
                Transaction.Dispose();
            }

            Transaction = CurrentSession.BeginTransaction();
        }

        private void should_not_currently_be_disposed()
        {
            if (_isDisposed) throw new ObjectDisposedException(GetType().Name);
        }

        private void should_be_initialized_first()
        {
            if (!_isInitialized) throw new InvalidOperationException("Must initialize (call Initialize()) on NHibernateUnitOfWork before commiting or rolling back");
        }

        public void Dispose()
        {
            if (_isDisposed || !_isInitialized) return;
            if (Transaction != null)
                Transaction.Dispose();
            if (CurrentSession != null)
                CurrentSession.Dispose();
            if (CurrentStatelessSession != null)
                CurrentStatelessSession.Dispose();
            _isDisposed = true;
        }
    }

}
