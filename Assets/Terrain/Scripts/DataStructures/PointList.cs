using UnityEngine;
using System.Collections.Generic;

public class PointList : List<Point> {
    public Vector3[] ToVector3Array() {
        Vector3[] array = new Vector3[Count];
        for (int i = 0; i < Count; ++i) {
            array[i] = this[i].position;
        }
        return array;
    }
    //public Color[] ToColorArray() {
    //    Color[] array = new Color[Count];
    //    for (int i = 0; i < Count; ++i) {
    //        array[i] = this[i].color;
    //    }
    //    return array;
    //}
}
