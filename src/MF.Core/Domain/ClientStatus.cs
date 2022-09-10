namespace MF.Core.Domain
{
    public class ClientStatus:DomainEntity
    {
        public virtual bool AdminAlerted { get; set; }
        public virtual bool ClientContacted { get; set; }
    }
}