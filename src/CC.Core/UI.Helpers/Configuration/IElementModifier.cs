namespace CC.Core.UI.Helpers.Configuration
{
    public interface IElementModifier
    {
        TagModifier CreateModifier(AccessorDef accessorDef);
    }
}
