using UnityEngine;

public enum ItemType
{
    Food,
    Clothing,
    Other
}

public class Item : Selectable
{
    public ItemType Type;
}