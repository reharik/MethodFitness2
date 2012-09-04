using MethodFitness.Core.Domain;

namespace MethodFitness.Web.Areas.Schedule.Controllers
{
    public class Location : DomainEntity, IPersistableObject
    {
        public virtual string Name { get; set; }
        public virtual string Address1 { get; set; }
        public virtual string Address2 { get; set; }
        public virtual string City { get; set; }
        public virtual string State { get; set; }
        public virtual string Zip { get; set; }
    }
}