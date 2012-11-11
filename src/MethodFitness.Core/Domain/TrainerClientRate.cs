using System;
using CC.Core;
using CC.Core.Domain;

namespace MethodFitness.Core.Domain
{
    public class TrainerClientRate :DomainEntity, IEquatable<TrainerClientRate>
    {
        public virtual Client Client { get; set; }
        public virtual User User { get; set; }
        public virtual int Percent { get; set; }

        public override void UpdateSelf(Entity entity)
        {
            var self = (TrainerClientRate)entity;
            Client = self.Client;
            User = self.User;
            Percent = self.Percent;
        }

        #region IEquatable

        public virtual bool Equals(TrainerClientRate obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (this.GetTypeWhenProxy() != obj.GetTypeWhenProxy()) return false;
            return (Client == obj.Client && User == obj.User && Percent == obj.Percent);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (this.GetTypeWhenProxy() != obj.GetTypeWhenProxy()) return false;
            return Equals((TrainerClientRate)obj);
        }

        public override int GetHashCode()
        {
            return EntityId.GetHashCode();
        }

        public static bool operator ==(TrainerClientRate left, TrainerClientRate right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TrainerClientRate left, TrainerClientRate right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}