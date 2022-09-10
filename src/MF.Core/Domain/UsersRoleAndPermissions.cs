using System;
using CC.Core.Core;
using CC.Core.Core.Domain;
using CC.Core.DataValidation.Attributes;

namespace MF.Core.Domain
{
    public class UsersRoleAndPermissions :DomainEntity, IEquatable<UsersRoleAndPermissions>
    {
        [DoNotValidate]
        public virtual UserRole Role { get; set; }
        [DoNotValidate]
        public virtual User Trainer { get; set; }
        public virtual Location LocationId { get; set; }

        #region IEquatable

        public virtual bool Equals(UsersRoleAndPermissions obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (this.GetTypeWhenProxy() != obj.GetTypeWhenProxy()) return false;
            return (Role == obj.Role && Trainer == obj.Trainer && LocationId == obj.LocationId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (this.GetTypeWhenProxy() != obj.GetTypeWhenProxy()) return false;
            return Equals((UsersRoleAndPermissions)obj);
        }

        public override int GetHashCode()
        {
            return EntityId.GetHashCode();
        }

        public static bool operator ==(UsersRoleAndPermissions left, UsersRoleAndPermissions right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(UsersRoleAndPermissions left, UsersRoleAndPermissions right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}