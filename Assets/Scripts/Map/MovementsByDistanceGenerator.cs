using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
 * Generates a list of all the possible movement coordinates 
 * (for example (0, -3) for moving down 3)
 * that could be made within a rectangle of the specified width and height
 * (for simplicity it uses a square that encompasses the rectangle)
 * sorted by distance from least to most distance
 *
 * Allows for nearest search by looping over the movement 
 * coordinates in order to search the nearest places first
 * 
 * Algorithm Credit: Dr Jack Fuller PhD 💅
 */
public static class MovementsByDistanceGenerator
{
    public static Vector2Int[] Generate(int width, int height)
    {
        Dictionary<int, List<Vector2Int>> dictionary = new();
        var max = Math.Max(width, height);

        // Add each possible coordinate to the dictionary keyed by the sum
        for (int x = 0; x < max; x++)
        {
            for (int y = 0; y <= x; y++)
            {
                int sum = x * x + y * y;
                if (!dictionary.ContainsKey(sum))
                    dictionary[sum] = new();
                // Add all possible coordinates in clockwise order
                dictionary[sum].Add(new Vector2Int(x, y));
                dictionary[sum].Add(new Vector2Int(x, y * -1));
                dictionary[sum].Add(new Vector2Int(x * -1, y * -1));
                dictionary[sum].Add(new Vector2Int(x * -1, y));
            }
        }

        return dictionary.OrderBy(kvp => kvp.Key).SelectMany(kvp => kvp.Value).ToArray();
    }
}