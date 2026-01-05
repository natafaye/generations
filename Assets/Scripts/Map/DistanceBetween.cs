using System;
using UnityEngine;

public static class Distance {
    public static double Between(Vector2 a, Vector2 b)
    {
        return Math.Sqrt(Math.Pow(a.x - b.x, 2) + Math.Pow(a.y - b.y, 2));
    }
}