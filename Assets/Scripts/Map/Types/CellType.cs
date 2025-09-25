using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "CellType", menuName = "ScriptableObjects/CellType")]
public class CellType : ScriptableObject
{
    public string Name;
    public bool Passable;
    public float WalkSpeed;
    public TileBase Tile;
}