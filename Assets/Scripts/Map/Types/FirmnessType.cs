using UnityEngine;

[CreateAssetMenu(fileName = "FirmnessType", menuName = "ScriptableObjects/FirmnessType")]
public class FirmnessType : ScriptableObject
{
    public StructureType Wall;
    public CellType Flat;
    public CellType Wet;
}