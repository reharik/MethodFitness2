using System.Configuration;

namespace MethodFitness.Core.Domain
{
    public class SessionRates:DomainEntity
    {
        

        public virtual double FullHour { get; set; }
        public virtual double HalfHour { get; set; }
        public virtual double FullHourTenPack { get; set; }
        public virtual double HalfHourTenPack { get; set; }
        public virtual double Pair { get; set; }
    }
}