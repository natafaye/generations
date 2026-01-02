using UnityEngine;

[CreateAssetMenu(fileName = "StructureType", menuName = "ScriptableObjects/StructureType")]
public class StructureType : EntityType
{
    public ItemType destroyProductType;
    public int destroyProductAmount;
}