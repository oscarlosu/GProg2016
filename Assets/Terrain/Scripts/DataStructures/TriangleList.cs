using System.Collections.Generic;

public class TriangleList : List<Triangle> {
    public int[] ToIndexArray() {
        int[] array = new int[3 * Count];
        for (int i = 0; i < Count; ++i) {
            array[3 * i] = this[i].v1;
            array[3 * i + 1] = this[i].v2;
            array[3 * i + 2] = this[i].v3;
        }
        return array;
    }


}
