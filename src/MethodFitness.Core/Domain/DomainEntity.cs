using System;
using MethodFitness.Core.Domain.Tools.CustomAttributes;
using MethodFitness.Core.Html.Grid;


namespace MethodFitness.Core.Domain
{
    public class DomainEntity : Entity
    {
        public virtual int CompanyId { get; set; }
    }

    public class Entity :  IGridEnabledClass, IEquatable<Entity>
    {
        public virtual int EntityId { get; set; }
        
        [ValidateSqlDateTime]
        public virtual DateTime? CreateDate { get; set; }

        [ValidateSqlDateTime]
        public virtual DateTime? ChangeDate { get; set; }

        
        public virtual int ChangedBy { get; set; }

        [System.ComponentModel.DefaultValue(0)] //pzt
        public virtual bool Archived { get; set; }

        public Entity()
        {
            
        }

        public virtual bool IsNew()
        {
            return EntityId == 0;
        }

        public virtual void UpdateSelf(Entity entity)
        {
            throw new NotImplementedException();
        }

        #region IEquatable

        public virtual bool Equals(Entity obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (this.GetTypeWhenProxy() != obj.GetTypeWhenProxy()) return false;
            return !IsNew() && obj.EntityId == EntityId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (this.GetTypeWhenProxy() != obj.GetTypeWhenProxy()) return false;
            return Equals((Entity) obj);
        }

        public override int GetHashCode()
        {
            return EntityId.GetHashCode();
        }

        public static bool operator ==(Entity left, Entity right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Entity left, Entity right)
        {
            return !Equals(left, right);
        }

        #endregion
        

    }

    public interface ILookupType
    {
        int EntityId { get; set; }
        string Name { get; set; }
    }
}