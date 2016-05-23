using System.Collections.Generic;
using UnityEngine;

public class TriangleList : List<Triangle> {
    public Vector3[] GetVector3ArrayRange(int firstTriangleIndex, int amount) {
        Vector3[] array = new Vector3[3 * amount];
        int index;
        for (int i = firstTriangleIndex; i < firstTriangleIndex + amount; ++i) {
            index = (i - firstTriangleIndex) * 3;
            array[index] = this[i].p1.position;
            array[index + 1] = this[i].p2.position;
            array[index + 2] = this[i].p3.position;
        }
        return array;
    }

    public int[] GetIndexArrayRange(int firstTriangleIndex, int amount) {
        int[] array = new int[3 * amount];
        for (int i = 0; i < amount; ++i) {
            array[3 * i] = 3 * i;
            array[3 * i + 1] = 3 * i + 1;
            array[3 * i + 2] = 3 * i + 2;
        }
        return array;
    }

    public Color[] GetColorArrayRange(int firstTriangleIndex, int amount) {
        Color[] array = new Color[3 * amount];
        int index;
        for (int i = firstTriangleIndex; i < firstTriangleIndex + amount; ++i) {
            index = (i - firstTriangleIndex) * 3;
            array[index] = this[i].color;
            array[index + 1] = this[i].color;
            array[index + 2] = this[i].color;
        }
        return array;
    }
}
