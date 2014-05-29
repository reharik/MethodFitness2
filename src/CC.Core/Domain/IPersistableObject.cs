namespace CC.Core.Domain
{
    public interface IPersistableObject:IReadableObject
    {
        bool IsDeleted { get; set; }
    }

    public interface IReadableObject
    {
    }

    public interface ILookupType
    {
        int EntityId { get; set; }
        string Name { get; set; }
    }
}