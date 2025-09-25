using UnityEngine;

// Based off of:
// https://gamedevacademy.org/procedural-2d-maps-unity-tutorial/

public class NoiseGenerator
{
    public static float[,] Generate(int width, int height, Wave[] waves, float scale = 1)
    {
        float[,] noiseMap = new float[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // calculate the sample positions
                float samplePosX = x * scale;
                float samplePosY = y * scale;

                float normalization = 0.0f;

                // loop through each wave
                foreach (Wave wave in waves)
                {
                    noiseMap[x, y] += wave.amplitude * Mathf.PerlinNoise(
                        samplePosX * wave.frequency + wave.seed, samplePosY * wave.frequency + wave.seed
                    );
                    normalization += wave.amplitude;
                }

                // normalize the value
                noiseMap[x, y] /= normalization;
            }
        }

        return noiseMap;
    }
}