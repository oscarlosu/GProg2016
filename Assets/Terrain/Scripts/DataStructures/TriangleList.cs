using System.Collections.Generic;
using UnityEngine;

public class TriangleList : List<Triangle> {
    public Vector3[] Vector3Array() {
        Vector3[] array = new Vector3[3 * Count];
        for (int i = 0; i < Count; ++i) {
            array[3 * i] = this[i].p1.position;
            array[3 * i + 1] = this[i].p2.position;
            array[3 * i + 2] = this[i].p3.position;
        }
        return array;
    }

    public int[] IndexArray() {
        int[] array = new int[3 * Count];
        for (int i = 0; i < Count; ++i) {
            array[3 * i] = 3 * i;
            array[3 * i + 1] = 3 * i + 1;
            array[3 * i + 2] = 3 * i + 2;
        }
        return array;
    }

    public Color[] ColorArray() {
        Color[] array = new Color[3 * Count];
        for (int i = 0; i < Count; ++i) {
            array[3 * i] = this[i].color;
            array[3 * i + 1] = this[i].color;
            array[3 * i + 2] = this[i].color;
        }
        return array;
    }
}
