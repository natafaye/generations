using UnityEngine;

[CreateAssetMenu(fileName = "ItemType", menuName = "ScriptableObjects/ItemType")]
public class ItemType : ScriptableObject, IEntityType
{
    [field: SerializeField]
    public string Name { get; set; }
    [field: SerializeField]
    public Sprite Sprite { get; set; }
    public ItemCategory category;
    public int stackSize;
    public int maxHealth;
}