using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator
{
    public static MapCell[,] Generate(
        int seed, int width, int height, BiomeType biome, Tilemap tilemap,
        EntityManager entityManager, FirmnessType[] firmnessTypes, CellType waterCellType
    ) {
        var heightMap = NoiseGenerator.Generate(seed, width, height, biome.HeightWaves);
        var firmnessMap = NoiseGenerator.Generate(seed, width, height, biome.FirmnessWaves);

        MapCell[,] cells = new MapCell[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                CellType cellType;
                StructureType structureType = null;

                // TODO: Outside of range of array
                var cellHeight = (int)MathF.Floor(heightMap[x, y] * 10);
                // Debug.Log("Cell Height: " + cellHeight);
                HeightLevel heightLevel = biome.MoistureType.Heights[cellHeight];
                
                var cellFirmness = (int)MathF.Floor(firmnessMap[x, y] * 3);
                // Debug.Log("Cell Firmness: " + cellFirmness);
                FirmnessType firmnessType = firmnessTypes[cellFirmness];

                var cellFertility = (int)MathF.Floor(firmnessMap[x, y] * 101);
                if (cellFirmness != 1) cellFertility = 0;

                switch (heightLevel)
                {
                    case HeightLevel.Roof:
                        cellType = firmnessType.Flat;
                        structureType = firmnessType.Wall;
                        break;
                    case HeightLevel.Wall:
                        cellType = firmnessType.Flat;
                        structureType = firmnessType.Wall;
                        break;
                    case HeightLevel.Rock:
                        cellType = firmnessType.Flat;
                        break;
                    case HeightLevel.Wet:
                        cellType = firmnessType.Wet;
                        break;
                    case HeightLevel.Water:
                        cellType = waterCellType;
                        break;
                    case HeightLevel.Deep:
                        cellType = waterCellType;
                        break;
                    default:
                        cellType = firmnessType.Flat;
                        var random = UnityEngine.Random.Range(1, 100);
                        if (random < cellFertility)
                        {
                            var randomIndex = UnityEngine.Random.Range(0, biome.StructureTypes.Length);
                            structureType = biome.StructureTypes[randomIndex];
                        }
                        break;
                }

                var mapPosition = new Vector2Int(x, y);
                var mapCell = new MapCell(cellType, mapPosition);
                cells[x, y] = mapCell;

                if (structureType != null)
                {
                    var structure = entityManager.Create(structureType);
                    mapCell.MoveToCell(structure);
                    if (structureType is PlantType type)
                    {
                        int maxAge = type.ageToStartHarvestCycle + (int)Math.Round(type.timeToFullHarvest * 1.5);
                        ((Plant)structure).Age = UnityEngine.Random.Range(0, maxAge);
                    }
                }

                tilemap.SetTile(new Vector3Int(x, y, 0), cellType.Tile);
            }
        }
        return cells;
    }
}