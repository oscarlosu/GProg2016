using UnityEngine;
using System;

[Serializable]
public class NoiseParams {
    public float Low = -1;
    public float High = 1;
    [Range(0, 100)]
    public float Frequency = 0.002f;
    [Range(0, 1)]
    public float Persistence = 0.8f;
    [Range(0, 10)]
    public int Octaves = 1;
}
