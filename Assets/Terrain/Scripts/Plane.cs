using UnityEngine;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Plane : MonoBehaviour {

    private MeshFilter mf;
    public MeshFilter Filter {
        get {
            if (mf == null) {
                mf = GetComponent<MeshFilter>();
            }
            return mf;
        }
    }

    private Mesh mesh;
    public Mesh Mesh {
        get {
            if (mesh == null) {
                mesh = new Mesh();
            }
            return mesh;
        }
        set {
            mesh = value;
        }
    }

    public PointList Points { get; set; }
    public TriangleList Triangles { get; set; }

    private Dictionary<Int64, int> middlePointIndexCache;

    [SerializeField]
    [Range(0, 7)]
    private int subdivisionSteps = 0;
    [SerializeField]
    private float sizeX = 1;
    [SerializeField]
    private float sizeZ = 1;

    private void InitialiseMesh() {
        // Accessors create assign Filter and mesh if the are null
        Filter.mesh = Mesh;
    }

    private void ClearMesh() {
        Mesh.Clear();
    }

    public void CreatePlane() {
        CreatePlane(this.sizeX, this.sizeZ, this.subdivisionSteps);
    }

    public void CreatePlane(float width, float depth, int subdivisionSteps) {

        this.sizeX = width;
        this.sizeZ = depth;
        this.subdivisionSteps = subdivisionSteps;
        // Clear lists
        Points = new PointList();
        Triangles = new TriangleList();


        // Vertices
        // XZ plane
        addVertex(new Point(sizeX / 2.0f, 0, -sizeZ / 2.0f));
        addVertex(new Point(sizeX / 2.0f, 0, sizeZ / 2.0f));
        addVertex(new Point(-sizeX / 2.0f, 0, -sizeZ / 2.0f));
        addVertex(new Point(-sizeX / 2.0f, 0, sizeZ / 2.0f));

        // Triangles
        Triangles.Add(new Triangle(1, 0, 2));
        Triangles.Add(new Triangle(3, 1, 2));

        Subdivide(subdivisionSteps);

        Debug.Log("Triangle count " + Triangles.Count + " Vertex count " + Points.Count);
    }

    private int addVertex(Point p) {
        Points.Add(p);
        return Points.Count - 1;
    }

    private int GetMiddlePoint(int p1, int p2) {
        // first check if we have it already
        bool firstIsSmaller = p1 < p2;
        Int64 smallerIndex = firstIsSmaller ? p1 : p2;
        Int64 greaterIndex = firstIsSmaller ? p2 : p1;
        Int64 key = (smallerIndex << 32) + greaterIndex;

        int ret;
        if (middlePointIndexCache.TryGetValue(key, out ret)) {
            return ret;
        }

        // not in cache, calculate it
        Point point1 = Points[p1];
        Point point2 = Points[p2];
        Point middle = new Point((point1.x + point2.x) / 2.0f,
                                     (point1.y + point2.y) / 2.0f,
                                     (point1.z + point2.z) / 2.0f);

        // add vertex makes sure point is on unit sphere
        int i = addVertex(middle);

        // store it in cache, return index
        middlePointIndexCache.Add(key, i);
        return i;
    }

    private void Subdivide(int subdivisionSteps) {
        middlePointIndexCache = new Dictionary<Int64, int>();

        for (int i = 0; i < subdivisionSteps; i++) {
            var triangles2 = new TriangleList();
            foreach (var tri in Triangles) {
                // replace triangle by 4 triangles
                int a = GetMiddlePoint(tri.v1, tri.v2);
                int b = GetMiddlePoint(tri.v2, tri.v3);
                int c = GetMiddlePoint(tri.v3, tri.v1);

                triangles2.Add(new Triangle(tri.v1, a, c));
                triangles2.Add(new Triangle(tri.v2, b, a));
                triangles2.Add(new Triangle(tri.v3, c, b));
                triangles2.Add(new Triangle(a, b, c));
            }
            Triangles = triangles2;
        }
    }

    public void UpdateMesh() {
        InitialiseMesh();
        ClearMesh();
        Mesh.vertices = Points.ToVector3Array();
        Mesh.colors = Points.ToColorArray();
        Mesh.triangles = Triangles.ToIndexArray();
        Mesh.RecalculateNormals();
    }
}

