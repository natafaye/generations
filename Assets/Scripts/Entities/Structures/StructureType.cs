using System;
using UnityEngine;

[Serializable]
public class ItemAmount
{
    public ItemType Type;
    public int Amount;
}

[CreateAssetMenu(fileName = "StructureType", menuName = "ScriptableObjects/StructureType/StructureType")]
public class StructureType : EntityType
{
    public ItemType destroyProductType;
    public int destroyProductAmount;

    // Sprite to use in the rotated position
    public Sprite RotatedSprite;

    // How many cells wide and tall (at 0 rotation)
    public Vector2Int CellSize = new(1, 1);

    // Materials needed to build
    public ItemAmount[] Materials;
    
    // Can this be claimed and used as a bed
    public bool IsBed = false;

    // What sprite to use (accounting for rotation)
    public Sprite GetRotatedSprite(int rotation)
    {
        return (RotatedSprite == null || rotation == 0 || rotation == 2) ? Sprite : RotatedSprite;
    }
}