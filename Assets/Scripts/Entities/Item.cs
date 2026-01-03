public class Item : Entity
{
    // Convenience property for getting the correctly typed Data
    public new ItemData Data { get { return (ItemData)base.Data; } }
}