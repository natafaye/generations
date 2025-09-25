using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct TypePercentage<T>
{
    public T Type;
    public float Percentage;
}

[Serializable]
public struct CellTypePercentage
{
    public CellType Type;
    public float Percentage;
    // Percentage chance of generating a particular structure type in each cell of this type
    // From lowest to highest like CellTypes
    // Last entry does not have to be 100, unused percentage is for blank cells
    public List<TypePercentage<StructureType>> StructureTypes;
}

[Serializable]
public class Wave
{
    public float seed;
    public float frequency;
    public float amplitude;
}

[CreateAssetMenu(fileName = "BiomeType", menuName = "ScriptableObjects/BiomeType")]
public class BiomeType : ScriptableObject
{
    public string Name;

    // In order from lowest to highest where the percentage 
    // is the upper limit of their range
    // Last entry should be 100
    // For example: 10, 30, 50, 80, 100
    // which would make the ranges: 1-10, 11-30, 31-50, 51-80, 81-100
    public List<CellTypePercentage> CellTypes;

    // Perlin Noise for height and fertility
    public Wave[] HeightWaves;
    public Wave[] FirmnessWaves;

    public MoistureType MoistureType;
}