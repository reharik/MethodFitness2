namespace MethodFitness.Core.Domain
{
    public class TrainerClientRate :DomainEntity
    {
        public virtual Client Client { get; set; }
        public virtual User Trainer { get; set; }
        public virtual int Percent { get; set; }
    }
}