using System;
using System.Collections.Generic;
using UnityEngine;

public static class MapUtilities {
    public static double DistanceBetween(Vector2 a, Vector2 b)
    {
        return Math.Sqrt(Math.Pow(a.x - b.x, 2) + Math.Pow(a.y - b.y, 2));
    }

    public static Vector2Int[] CellIndexesInArea(Vector2Int area, Vector2Int cornerIndex, int rotation = 0)
    {
        List<Vector2Int> cellIndexes = new();
        for(int x = 0; x < area.x; x++)
        {
            for(int y = 0; y < area.y; y++)
            {
                Vector2Int offset;
                if(rotation == 0) offset = new(x, -y);
                else if(rotation == 1) offset = new(-y, -x);
                else if(rotation == 2) offset = new(-x, y);
                else offset = new (y, x);
                cellIndexes.Add(new(cornerIndex.x + offset.x, cornerIndex.y + offset.y));
            }
        }
        return cellIndexes.ToArray();
    }
}