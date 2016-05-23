using UnityEngine;

public class Triangle {
    private Point[] points;

    public Point p1 {
        get {
            return points[0];
        }
        set {
            points[0] = value;
        }
    }
    public Point p2 {
        get {
            return points[1];
        }
        set {
            points[1] = value;
        }
    }
    public Point p3 {
        get {
            return points[2];
        }
        set {
            points[2] = value;
        }
    }

    public Color color;

    public Triangle(Point p1, Point p2, Point p3) {
        points = new Point[3];
        this.p1 = p1.Copy();
        this.p2 = p2.Copy();
        this.p3 = p3.Copy();
        this.color = new Color(1, 0, 1, 1); // Default color, chose pink mainly for debugging purposes
    }

}
