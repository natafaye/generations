public class ItemData : EntityData
{
    // Convenience property for getting the correctly typed Type
    public new ItemType Type { get { return (ItemType)base.Type; } }

    public int ItemsInStack;

    public ItemData(EntityType type, int itemsInStack) : base(type)
    {
        ItemsInStack = itemsInStack;
    }
}