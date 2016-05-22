using UnityEngine;
using System.Collections;

public static class Noise {
    // [-1, 1]
	public static float Fractal(float x, float y, float frequency, float persistence, int octaves, float offset) {
        float total = 0;
        float maxAmplitude = 0;
        float amplitude = 1;
        // Calculate noise octaves
        for(int i = 0; i < octaves; ++i) {
            // Sample Perlin noise
            total += (Mathf.PerlinNoise(offset + x * frequency, offset + y * frequency) * 2.0f - 1.0f) * amplitude;
            // Halve wavelength = double frequency
            frequency *= 2;
            // Add maximum possible contribution from octave
            maxAmplitude += amplitude;
            // Reduce amplitude according to persistence
            amplitude *= persistence;
        }
        // Normalize to [0, 1]
        return total / maxAmplitude;
    }

    // [0, 1] (Bubbly noise)
    public static float AbsFractal(float x, float y, float frequency, float persistence, int octaves, float offset) {
        float total = 0;
        float maxAmplitude = 0;
        float amplitude = 1;
        // Calculate noise octaves
        for (int i = 0; i < octaves; ++i) {
            // Sample Perlin noise
            total += Mathf.Abs((Mathf.PerlinNoise(offset + x * frequency, offset + y * frequency) * 2.0f - 1.0f)) * amplitude;
            // Halve wavelength = double frequency
            frequency *= 2;
            // Add maximum possible contribution from octave
            maxAmplitude += amplitude;
            // Reduce amplitude according to persistence
            amplitude *= persistence;
        }
        // Normalize to [0, 1]
        return total / maxAmplitude;
    }

    // [-1, 1] (Ridged Mountains)
    public static float RidgedFractal(float x, float y, float frequency, float persistence, int octaves, float offset) {
        float total = 0;
        float maxAmplitude = 0;
        float amplitude = 1;
        float perlin, ridged;
        // Calculate noise octaves
        for (int i = 0; i < octaves; ++i) {
            // Perlin in [-1, 1]
            perlin = Mathf.PerlinNoise(offset + x * frequency, offset + y * frequency) * 2.0f - 1.0f;
            ridged = ((1.0f - Mathf.Abs(perlin)) * 2.0f - 1.0f);
            total += ridged * amplitude;
            // Halve wavelength = double frequency
            frequency *= 2;
            // Add maximum possible contribution from octave
            maxAmplitude += amplitude;
            // Reduce amplitude according to persistence
            amplitude *= persistence;
        }
        // Normalize to [0, 1]
        return total / maxAmplitude;
    }

    // [0, 1] (Squared fractal - smooth, with smoothed ridges?)
    public static float SquaredFractal(float x, float y, float frequency, float persistence, int octaves, float offset) {
        float total = 0;
        float maxAmplitude = 0;
        float amplitude = 1;
        float perlin;
        // Calculate noise octaves
        for (int i = 0; i < octaves; ++i) {
            // Sample Perlin noise
            perlin = Mathf.PerlinNoise(offset + x * frequency, offset + y * frequency) * 2.0f - 1.0f;
            total += perlin * perlin * amplitude;
            // Halve wavelength = double frequency
            frequency *= 2;
            // Add maximum possible contribution from octave
            maxAmplitude += amplitude;
            // Reduce amplitude according to persistence
            amplitude *= persistence;
        }
        // Normalize to [0, 1]
        return total / maxAmplitude;
    }

    // [-1, 1] (Cubed fractal - smooth)
    public static float CubedFractal(float x, float y, float frequency, float persistence, int octaves, float offset) {
        float total = 0;
        float maxAmplitude = 0;
        float amplitude = 1;
        float perlin;
        // Calculate noise octaves
        for (int i = 0; i < octaves; ++i) {
            // Sample Perlin noise
            perlin = Mathf.PerlinNoise(offset + x * frequency, offset + y * frequency) * 2.0f - 1.0f;
            total += perlin * perlin * perlin * amplitude;
            // Halve wavelength = double frequency
            frequency *= 2;
            // Add maximum possible contribution from octave
            maxAmplitude += amplitude;
            // Reduce amplitude according to persistence
            amplitude *= persistence;
        }
        // Normalize to [0, 1]
        return total / maxAmplitude;
    }
}
