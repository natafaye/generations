using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator
{
    public static MapCell[,] Generate(
        int width, int height, BiomeType biome, Tilemap tilemap,
        EntityManager entityManager, FirmnessType[] firmnessTypes,
        CellType waterCellType
    ) {
        var heightMap = NoiseGenerator.Generate(width, height, biome.HeightWaves);
        var firmnessMap = NoiseGenerator.Generate(width, height, biome.FirmnessWaves);

        MapCell[,] cells = new MapCell[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                CellType cellType;
                StructureType structureType = null;

                var cellHeight = (int)MathF.Round(heightMap[x, y] * 10);
                HeightLevel heightLevel = biome.MoistureType.Heights[cellHeight];
                
                var cellFirmness = (int)MathF.Round(firmnessMap[x, y] * 3);
                FirmnessType firmnessType = firmnessTypes[cellFirmness];

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
                        break;
                }

                // var r1 = UnityEngine.Random.Range(1, 100);
                // foreach (var cellPercentageType in biome.CellTypes)
                // {
                //     if (r1 <= cellPercentageType.Percentage)
                //     {
                //         cellType = cellPercentageType.Type;
                //         var r2 = UnityEngine.Random.Range(1, 100);
                //         foreach (var sType in cellPercentageType.StructureTypes)
                //         {
                //             if (r2 <= sType.Percentage)
                //             {
                //                 structureType = sType.Type;
                //                 break;
                //             }
                //         }
                //         break;
                //     }
                // }

                var mapPosition = new Vector2Int(x, y);
                var mapCell = new MapCell(cellType, mapPosition);
                cells[x, y] = mapCell;

                if (structureType != null)
                {
                    var structure = entityManager.Create(structureType);
                    mapCell.MoveToCell(structure);
                }

                tilemap.SetTile(new Vector3Int(x, y, 0), cellType.Tile);
            }
        }
        return cells;
    }
}

// fertility (more or less growth)
// height (low enough for water, high enough for mountains)
// heat (snow or grass or sand)