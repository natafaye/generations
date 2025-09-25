using UnityEngine;

public enum ItemCategory
{
    Food,
    Clothing,
    Other
}

[RequireComponent(typeof(SpriteRenderer))]
public class Item : MonoBehaviour, IEntity
{
    [field: SerializeField]
    public string Name { get; set; }

    [field: SerializeField]
    public ItemType Type { get; set; }
    // Handle type inheritance of IEntityType to ItemType
    IEntityType IEntity.Type { get => Type; set => Type = (ItemType)value; }

    public bool IsSelected { get; set; }

    public Vector2Int MapPosition { get; set; }
    public Transform Transform { get; set; }
    public SpriteRenderer SpriteRenderer { get; set; }

    public int ItemsInStack { get; set; }

    public Item(ItemType type, int itemsInStack)
    {
        Type = type;
        Name = type.Name;
        ItemsInStack = itemsInStack;
        Transform = transform;
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }
}