namespace MethodFitness.Core.Domain
{
    public class TrainerClientRate :DomainEntity
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
    }
}