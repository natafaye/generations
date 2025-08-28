using UnityEngine;

public enum ItemType
{
    Food,
    Clothing,
    Other
}

public class Item : MonoBehaviour
{
    public ItemType Type;
}