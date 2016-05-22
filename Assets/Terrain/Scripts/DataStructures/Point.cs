using UnityEngine;

public class Point {
    public Vector3 position;    

    public float x {
        get {
            return position.x;
        }
        set {
            position.x = value;
        }
    }
    public float y {
        get {
            return position.y;
        }
        set {
            position.y = value;
        }
    }
    public float z {
        get {
            return position.z;
        }
        set {
            position.z = value;
        }
    }

    public Point(float x, float y, float z) {
        this.position = new Vector3(x, y, z);
    }

    public Point Copy() {
        return new Point(x, y, z);
    }
}
