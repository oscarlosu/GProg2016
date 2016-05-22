using UnityEngine;

public class Point {
    public Vector3 position;
    public Color color;

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
        this.color = Color.red;
    }

    public Point(float x, float y, float z, Color color) {
        this.position = new Vector3(x, y, z);
        this.color = color;
    }

    public Point(float x, float y, float z, float r, float g, float b, float a) {
        this.position = new Vector3(x, y, z);
        this.color = new Color(r, g, b, a);
    }
}
