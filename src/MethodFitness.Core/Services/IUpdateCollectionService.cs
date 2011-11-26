using System;
using System.Collections.Generic;
using System.Linq;
using MethodFitness.Core.Domain;

namespace MethodFitness.Core.Services
{
    public interface IUpdateCollectionService
    {
        void Update<ENTITY>(IEnumerable<ENTITY> origional,
                    IEnumerable<ENTITY> newItems,
                    Action<ENTITY> addEntity,
                    Action<ENTITY> removeEntity) where ENTITY : Entity;
    }

    public class UpdateCollectionService : IUpdateCollectionService 
    {
        public void Update<ENTITY>(IEnumerable<ENTITY> origional,
            IEnumerable<ENTITY> newItems,
            Action<ENTITY> addEntity,
            Action<ENTITY> removeEntity) where ENTITY : Entity 
        {
            var remove = new List<ENTITY>();
            origional.Each(x =>
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
            remove.Each(removeEntity);
            newItems.Each(x =>
            {
                if (!origional.Contains(x))
                {
                    addEntity(x);
                }
            });
        }
    }
}