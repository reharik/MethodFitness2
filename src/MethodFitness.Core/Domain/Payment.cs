namespace MethodFitness.Core.Domain
{
    public class Payment:DomainEntity
    {
        public virtual int FullHours { get; set; }
        public virtual int HalfHours { get; set; }
        public virtual int FullHourTenPacks { get; set; }
        public virtual int HalfHourTenPacks { get; set; }
        public virtual int Pairs { get; set; }
        public virtual double PaymentTotal { get; set; }
        public virtual Client Client { get; set; }
    }
}