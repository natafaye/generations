public class Structure : Entity
{
    // Convenience property for getting Data as StructureData
    public new StructureData Data { get { return (StructureData)base.Data; } }

    protected JobResult DestroyEntity()
    {
        GameManager.Instance.DestroyEntity(this);
        return new JobResult() { 
            type = Data.Type.destroyProductType, 
            amount = Data.Type.destroyProductAmount 
        };
    }

    protected override void OnDataChange()
    {
        base.OnDataChange();
        Overlay.sprite = Data.QueuedJob?.TypeData.Sprite;
    }
    
}