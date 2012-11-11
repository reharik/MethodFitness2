using System;
using CC.Core.Domain;

namespace MethodFitness.Core.Domain
{
    public class Payment:DomainEntity
    {
        public virtual Guid PaymentBatchId { get; set; }
        public virtual Client Client { get; set; }
        public virtual int FullHour { get; set; }
        public virtual double FullHourPrice { get; set; }
        public virtual int HalfHour { get; set; }
        public virtual double HalfHourPrice { get; set; }
        public virtual int FullHourTenPack { get; set; }
        public virtual double FullHourTenPackPrice { get; set; }
        public virtual int HalfHourTenPack { get; set; }
        public virtual double HalfHourTenPackPrice { get; set; }
        public virtual int Pair { get; set; }
        public virtual double PairPrice { get; set; }
        public virtual double PaymentTotal { get; set; }
        public virtual int PairTenPack { get; set; }
        public virtual double PairTenPackPrice { get; set; }
    }
}