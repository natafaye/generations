public class StructureData : EntityData
{
    // Convenience property for getting the correctly typed Type
    public new StructureType Type { get { return (StructureType)base.Type; } }

    public StructureData(StructureType type) : base(type)
    {
        
    }
}