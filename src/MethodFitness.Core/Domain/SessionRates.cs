using System.Configuration;
using CC.Core.Domain;

namespace MethodFitness.Core.Domain
{
    public class SessionRates:DomainEntity
    {
        public virtual double FullHour { get; set; }
        public virtual double HalfHour { get; set; }
        public virtual double FullHourTenPack { get; set; }
        public virtual double HalfHourTenPack { get; set; }
        public virtual double Pair { get; set; }
        public virtual double PairTenPack { get; set; }

        public SessionRates()
        {
        }

        public SessionRates(BaseSessionRate rates)
        {
            FullHour = rates.FullHour;
            HalfHour = rates.HalfHour;
            FullHourTenPack = rates.FullHourTenPack;
            HalfHourTenPack = rates.HalfHourTenPack;
            Pair = rates.Pair;
            PairTenPack = rates.PairTenPack;
        }
    }
}