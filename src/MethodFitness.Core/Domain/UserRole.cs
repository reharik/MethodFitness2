namespace MethodFitness.Core.Domain
{
    public class UserRole:Entity
    {
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public override void UpdateSelf(Entity entity)
        {
            var userRole = (UserRole) entity;
            Name = userRole.Name;
            Description = userRole.Description;
        }
    }
}