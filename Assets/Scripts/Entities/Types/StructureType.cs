using UnityEngine;

public enum StructureCategory
{
    Plant,
    Wall,
    Bed
}

[CreateAssetMenu(fileName = "StructureType", menuName = "ScriptableObjects/StructureType")]
public class StructureType : ScriptableObject, IEntityType
{
    [field: SerializeField]
    public string Name { get; set; }
    [field: SerializeField]
    public Sprite Sprite { get; set; }

    public StructureCategory category;

    public ItemType destroyProductType;
    public int destroyProductAmount;

    public int maxHealth;
}