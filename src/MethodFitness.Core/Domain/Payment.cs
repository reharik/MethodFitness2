using System;

namespace MethodFitness.Core.Domain
{
    public class Payment:DomainEntity
    {
        public virtual int FullHours { get; set; }
        public virtual double FullHoursPrice { get; set; }
        public virtual int HalfHours { get; set; }
        public virtual double HalfHoursPrice { get; set; }
        public virtual int FullHourTenPacks { get; set; }
        public virtual double FullHourTenPacksPrice { get; set; }
        public virtual int HalfHourTenPacks { get; set; }
        public virtual double HalfHourTenPacksPrice { get; set; }
        public virtual int Pairs { get; set; }
        public virtual double PairsPrice { get; set; }
        public virtual double PaymentTotal { get; set; }
        public virtual Guid PaymentBatchId { get; set; }
        public virtual Client Client { get; set; }
    }
}