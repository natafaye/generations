using UnityEngine;

public enum HeightLevel
{
    Roof, Wall, Rock, Flat, Wet, Water, Deep
}

[CreateAssetMenu(fileName = "MoistureType", menuName = "ScriptableObjects/MoistureType")]
public class MoistureType : ScriptableObject
{
    public HeightLevel[] Heights;
    public int Precipitation;
}