namespace MethodFitness.Core.Domain
{
    public interface IPersistableObject
    {
        bool Archived { get; set; }
    }
}