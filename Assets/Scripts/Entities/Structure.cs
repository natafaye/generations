public class Structure : Entity
{
    // Convenience property for getting Type as StructureType
    public StructureType StructureType { get { return (StructureType)Type; } }

    protected JobResult DestroyEntity()
    {
        GameManager.Instance.DestroyEntity(this);
        return new JobResult() { 
            type = StructureType.destroyProductType, 
            amount = StructureType.destroyProductAmount 
        };
    }
    
}