using CC.Core.Domain;
using Castle.Components.Validator;

namespace MethodFitness.Core.Domain
{
    public class BaseSessionRate : DomainEntity, IPersistableObject
    {
        [ValidateNonEmpty]
        public virtual double FullHour { get; set; }
        [ValidateNonEmpty]
        public virtual double HalfHour { get; set; }
        [ValidateNonEmpty]
        public virtual double FullHourTenPack { get; set; }
        [ValidateNonEmpty]
        public virtual double HalfHourTenPack { get; set; }
        [ValidateNonEmpty]
        public virtual double Pair { get; set; }
        [ValidateNonEmpty]
        public virtual double PairTenPack { get; set; }
    }
}