namespace CC.UI.Helpers.Configuration
{
    public interface IElementModifier
    {
        TagModifier CreateModifier(AccessorDef accessorDef);
    }
}
