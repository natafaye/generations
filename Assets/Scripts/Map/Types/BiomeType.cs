using System;
using UnityEngine;

[Serializable]
public class Wave
{
    public float frequency;
    public float amplitude;
}

[CreateAssetMenu(fileName = "BiomeType", menuName = "ScriptableObjects/BiomeType")]
public class BiomeType : ScriptableObject
{
    public string Name;

    public Wave[] HeightWaves;
    public Wave[] FirmnessWaves;
    public Wave[] FertilityWaves;

    public MoistureType MoistureType;

    public StructureType[] StructureTypes;
}