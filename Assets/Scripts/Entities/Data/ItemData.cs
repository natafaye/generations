public class ItemData : EntityData
{
    // Convenience property for getting the correctly typed Type
    public new ItemType Type { get { return (ItemType)base.Type; } }

    private int _itemsInStack;
    public int ItemsInStack
    {
        get { return _itemsInStack; }
        set
        {
            if(value == _itemsInStack) return;
            _itemsInStack = value;
            OnChange?.Invoke();
        }
    }

    public ItemData(ItemType type, int itemsInStack) : base(type)
    {
        ItemsInStack = itemsInStack;
    }
}