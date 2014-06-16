using System;
using System.Collections.Generic;
using System.Linq;
using CC.Core.CoreViewModelAndDTOs;
using CC.Core.Domain;
using CC.Core.DomainTools;

namespace CC.Core.Services
{
    public interface IUpdateCollectionService
    {
        void UpdateCollectionDetails<ENTITY>(IEnumerable<ENTITY> origional,
                    IEnumerable<ENTITY> newItems,
                    Action<ENTITY> addEntity,
                    Action<ENTITY> removeEntity) where ENTITY : Entity;

        // distinguisher is for when you have a collection of a base class
        // and you don't want to delete the other types just the one you're working with
        void Update<ENTITY>(IEnumerable<ENTITY> origional,
                            TokenInputViewModel tokenInputViewModel,
                            Action<ENTITY> addEntity,
                            Action<ENTITY> removeEntity,
            LambdaComparer<ENTITY> comparer = null) where ENTITY : Entity, IPersistableObject;
    }

    public class UpdateCollectionService : IUpdateCollectionService
    {
        private readonly IRepository _repository;

        public UpdateCollectionService(IRepository repository)
        {
            _repository = repository;
        }

        public void UpdateCollectionDetails<ENTITY>(IEnumerable<ENTITY> origional,
            IEnumerable<ENTITY> newItems,
            Action<ENTITY> addEntity,
            Action<ENTITY> removeEntity) where ENTITY : Entity
        {
            var remove = new List<ENTITY>();
            origional.ForEachItem(x =>
            {
                var newItem = newItems.FirstOrDefault(i => i.EntityId == x.EntityId);
                if (newItem == null)
                {
                    remove.Add(x);
                }
                else
                {
                    x.UpdateSelf(newItem);
                }
            });
            remove.ForEachItem(removeEntity);
            newItems.ForEachItem(x =>
            {
                if (!origional.Contains(x))
                {
                    addEntity(x);
                }
            });
        }

        public void Update<ENTITY>(IEnumerable<ENTITY> origional,
            TokenInputViewModel tokenInputViewModel,
            Action<ENTITY> addEntity,
            Action<ENTITY> removeEntity,
            LambdaComparer<ENTITY> comparer = null) where ENTITY : Entity, IPersistableObject
        {
            if (comparer == null)
            {
                comparer = new LambdaComparer<ENTITY>((a, b) => a.EntityId == b.EntityId);
            }
            var newItems = new List<ENTITY>();
            if (tokenInputViewModel != null && tokenInputViewModel.selectedItems != null)
            { tokenInputViewModel.selectedItems.ForEachItem(x => newItems.Add(_repository.Find<ENTITY>(Int32.Parse(x.id)))); }

            List<ENTITY> remove = origional.Except(newItems, comparer).ToList();

            remove.ForEachItem(removeEntity);
            newItems.ForEachItem(x =>
            {
                if (!origional.Contains(x))
                {
                    addEntity(x);
                }
            });
        }
    }

    public class LambdaComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, T, bool> _lambdaComparer;
        private readonly Func<T, int> _lambdaHash;

        public LambdaComparer(Func<T, T, bool> lambdaComparer) :
            this(lambdaComparer, o => 0)
        {
        }

        public LambdaComparer(Func<T, T, bool> lambdaComparer, Func<T, int> lambdaHash)
        {
            if (lambdaComparer == null)
                throw new ArgumentNullException("lambdaComparer");
            if (lambdaHash == null)
                throw new ArgumentNullException("lambdaHash");

            _lambdaComparer = lambdaComparer;
            _lambdaHash = lambdaHash;
        }

        public bool Equals(T x, T y)
        {
            return _lambdaComparer(x, y);
        }

        public int GetHashCode(T obj)
        {
            return _lambdaHash(obj);
        }
    }
}