using CC.Core.Domain;

namespace MF.Core.Domain
{
    public class Company:DomainEntity,IPersistableObject
    {
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
    }
}