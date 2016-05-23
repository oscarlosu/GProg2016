using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

[Serializable]
public class Area {
    public List<Color> palette;
    public float endElevation;
    public UnityEvent<Point> distribution;

    public Color GetColor(Point p) {
        //distribution.Invoke(p);
        return palette[UnityEngine.Random.Range(0, palette.Count - 1)];
    }
}
