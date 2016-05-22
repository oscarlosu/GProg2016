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

    public TriangleList Triangles { get; set; }

    [SerializeField]
    [Range(0, 6)]
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
        Triangles = new TriangleList();


        // Vertices
        // XZ plane
        Point p1 = new Point(sizeX / 2.0f, 0, -sizeZ / 2.0f);
        Point p2 = new Point(sizeX / 2.0f, 0, sizeZ / 2.0f);
        Point p3 = new Point(-sizeX / 2.0f, 0, -sizeZ / 2.0f);
        Point p4 = new Point(-sizeX / 2.0f, 0, sizeZ / 2.0f);

        // Triangles
        Triangles.Add(new Triangle(p2, p1, p3));
        Triangles.Add(new Triangle(p4, p2, p3));

        Subdivide(subdivisionSteps);
    }

    private Point GetMiddlePoint(Point a, Point b) {
        // Calculate
        return new Point((a.x + b.x) / 2.0f,
                                (a.y + b.y) / 2.0f,
                                (a.z + b.z) / 2.0f);
    }

    private void Subdivide(int subdivisionSteps) {

        for (int i = 0; i < subdivisionSteps; ++i) {
            TriangleList triangles2 = new TriangleList();
            for (int j = 0; j < Triangles.Count; ++j) {
                // replace triangle by 4 triangles
                Point p1p2 = GetMiddlePoint(Triangles[j].p1, Triangles[j].p2);
                Point p2p3 = GetMiddlePoint(Triangles[j].p2, Triangles[j].p3);
                Point p3p1 = GetMiddlePoint(Triangles[j].p3, Triangles[j].p1);

                triangles2.Add(new Triangle(Triangles[j].p1, p1p2, p3p1));
                triangles2.Add(new Triangle(Triangles[j].p2, p2p3, p1p2));
                triangles2.Add(new Triangle(Triangles[j].p3, p3p1, p2p3));
                triangles2.Add(new Triangle(p1p2, p2p3, p3p1));
            }
            Triangles = triangles2;
        }
    }

    public void UpdateMesh() {
        InitialiseMesh();
        ClearMesh();
        Mesh.vertices = Triangles.Vector3Array();
        Mesh.colors = Triangles.ColorArray();
        Mesh.triangles = Triangles.IndexArray();
        Mesh.RecalculateNormals();
    }
}

