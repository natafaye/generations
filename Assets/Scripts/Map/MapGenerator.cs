using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Generations {
    public class MapGenerator
    {
        public static MapCell[,] Generate(
            MapCell[,] cells, MapManager map, int seed, int width, int height, BiomeType biome, Tilemap tilemap,
            FirmnessType[] firmnessTypes, CellType waterCellType
        ) {
            var heightMap = NoiseGenerator.Generate(seed, width, height, biome.HeightWaves);
            var firmnessMap = NoiseGenerator.Generate(seed, width, height, biome.FirmnessWaves);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    CellType cellType;
                    StructureType structureType = null;

                    // TODO: Outside of range of array
                    var cellHeight = (int)MathF.Floor(heightMap[x, y] * 9);
                    // Debug.Log("Cell Height: " + cellHeight);
                    HeightLevel heightLevel = biome.MoistureType.Heights[cellHeight];
                    
                    var cellFirmness = (int)MathF.Floor(firmnessMap[x, y] * 2);
                    // Debug.Log("Cell Firmness: " + cellFirmness);
                    FirmnessType firmnessType = firmnessTypes[cellFirmness];

                    var cellFertility = (int)MathF.Floor(firmnessMap[x, y] * 100);
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
                    var mapCell = new MapCell(cellType, mapPosition, map);
                    cells[x, y] = mapCell;

                    if (structureType != null)
                    {
                        StructureData data = new(structureType);
                        if (structureType is PlantType plantType)
                        {
                            int maxAge = plantType.ageToStartHarvestCycle + (int)Math.Round(plantType.timeToFullHarvest * 1.5);
                            int randomAge = UnityEngine.Random.Range(0, maxAge);
                            data = new PlantData(plantType, randomAge);
                        }
                        data.MapPosition = mapCell.MapPosition;
                        EntityEvents.RequestCreateEntity.Invoke(data);
                    }

                    tilemap.SetTile(new Vector3Int(x, y, 0), cellType.Tile);
                }
            }
            return cells;
        }
    }
}