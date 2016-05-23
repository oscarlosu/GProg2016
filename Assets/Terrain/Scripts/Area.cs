using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

[Serializable]
public class Area {
    public string Name;
    public List<Color> Palette;
    public float EndElevation;
    //public UnityEvent<Triangle> distribution;

    public Color GetColor(Triangle tri) {
        //distribution.Invoke(p);
        return Palette[UnityEngine.Random.Range(0, Palette.Count)];
    }
}
