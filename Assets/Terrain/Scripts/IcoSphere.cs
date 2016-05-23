using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class IcoSphere : MonoBehaviour {

    private const int maxTrianglesInMesh = 20000;
    private List<Mesh> meshes;
    public List<Mesh> Meshes {
        get {
            if (meshes == null) {
                meshes = new List<Mesh>();
            }
            return meshes;
        }
        set {
            meshes = value;
        }
    }

    private List<GameObject> meshHolders;
    public List<GameObject> MeshHolders {
        get {
            if (meshHolders == null) {
                meshHolders = new List<GameObject>();
            }
            return meshHolders;
        }
        set {
            meshHolders = value;
        }
    }

    //public PointList Points { get; set; }
    public TriangleList Triangles { get; set; }

    [SerializeField]
    [Range(0, 9)]
    private int subdivisionSteps = 0;
    [SerializeField]
    private float radius = 1;

    [SerializeField]
    private Material defaultMat;

    public void ClearMesh() {
        for (int i = MeshHolders.Count - 1; i >= 0; --i) {
            DestroyImmediate(MeshHolders[i]);
        }
        meshHolders.Clear();
    }

    public void CreateIcosphere() {
        CreateIcosphere(this.radius, this.subdivisionSteps);
    }

    public void CreateIcosphere(float radius, int subdivisionSteps) {

        this.radius = radius;
        this.subdivisionSteps = subdivisionSteps;
        // Clear lists
        Triangles = new TriangleList();

        var t = (1.0f + Mathf.Sqrt(5.0f)) / 2.0f;
        // Vertices
        // XY plane
        Point xy_1 = AdjustPosition(new Point(-1, t, 0));
        Point xy_2 = AdjustPosition(new Point(1, t, 0));
        Point xy_3 = AdjustPosition(new Point(-1, -t, 0));
        Point xy_4 = AdjustPosition(new Point(1, -t, 0));
        // YZ plane
        Point yz_1 = AdjustPosition(new Point(0, -1, t));
        Point yz_2 = AdjustPosition(new Point(0, 1, t));
        Point yz_3 = AdjustPosition(new Point(0, -1, -t));
        Point yz_4 = AdjustPosition(new Point(0, 1, -t));
        // XZ plane
        Point xz_1 = AdjustPosition(new Point(t, 0, -1));
        Point xz_2 = AdjustPosition(new Point(t, 0, 1));
        Point xz_3 = AdjustPosition(new Point(-t, 0, -1));
        Point xz_4 = AdjustPosition(new Point(-t, 0, 1));

        // Triangles
        Triangles.Add(new Triangle(xy_1, xz_4, yz_2));
        Triangles.Add(new Triangle(xy_1, yz_2, xy_2));
        Triangles.Add(new Triangle(xy_1, xy_2, yz_4));
        Triangles.Add(new Triangle(xy_1, yz_4, xz_3));
        Triangles.Add(new Triangle(xy_1, xz_3, xz_4));

        // 5 adjacent triangles
        Triangles.Add(new Triangle(xy_2, yz_2, xz_2));
        Triangles.Add(new Triangle(yz_2, xz_4, yz_1));
        Triangles.Add(new Triangle(xz_4, xz_3, xy_3));
        Triangles.Add(new Triangle(xz_3, yz_4, yz_3));
        Triangles.Add(new Triangle(yz_4, xy_2, xz_1));

        // 5 triangles around point 3
        Triangles.Add(new Triangle(xy_4, xz_2, yz_1));
        Triangles.Add(new Triangle(xy_4, yz_1, xy_3));
        Triangles.Add(new Triangle(xy_4, xy_3, yz_3));
        Triangles.Add(new Triangle(xy_4, yz_3, xz_1));
        Triangles.Add(new Triangle(xy_4, xz_1, xz_2));

        // 5 adjacent triangles
        Triangles.Add(new Triangle(yz_1, xz_2, yz_2));
        Triangles.Add(new Triangle(xy_3, yz_1, xz_4));
        Triangles.Add(new Triangle(yz_3, xy_3, xz_3));
        Triangles.Add(new Triangle(xz_1, yz_3, yz_4));
        Triangles.Add(new Triangle(xz_2, xz_1, xy_2));


        Subdivide(subdivisionSteps);
    }

    private Point AdjustPosition(Point p) {
        float length = Mathf.Sqrt(p.position.x * p.position.x + p.position.y * p.position.y + p.position.z * p.position.z);
        p.position = new Vector3(p.position.x / length, p.position.y / length, p.position.z / length) * radius;
        return p;
    }

    private Point GetMiddlePoint(Point a, Point b) {
        // Calculate
        Point middle = new Point((a.x + b.x) / 2.0f,
                                (a.y + b.y) / 2.0f,
                                (a.z + b.z) / 2.0f);

        // make sure point is on unit sphere
        return AdjustPosition(middle);
    }

    private void Subdivide(int subdivisionSteps) {
        for (int i = 0; i < subdivisionSteps; ++i) {
            TriangleList triangles2 = new TriangleList();
            for(int j = 0; j < Triangles.Count; ++j) {
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
        UpdateMesh(defaultMat);
    }

    public void UpdateMesh(Material mat) {
        ClearMesh();
        // Figure out how many meshes will be necessary
        int nMeshes = (int)Mathf.Ceil((float)(Triangles.Count * 3.0f) / (float)maxTrianglesInMesh);
        // Initialise counter
        int added = 0;

        while (MeshHolders.Count < nMeshes) {
            GameObject holder = new GameObject();
            holder.transform.parent = transform;
            // Add Mesh Filter and Mesh Renderer
            MeshFilter filter = holder.AddComponent<MeshFilter>();
            MeshRenderer rend = holder.AddComponent<MeshRenderer>();
            // Set material for renderer
            rend.material = mat;
            // Create mesh and set in Mesh Filter
            Mesh mesh = new Mesh();
            int toBeAdded = Mathf.Min(maxTrianglesInMesh, Triangles.Count - added);
            mesh.vertices = Triangles.GetVector3ArrayRange(added, toBeAdded);
            mesh.colors = Triangles.GetColorArrayRange(added, toBeAdded);
            mesh.triangles = Triangles.GetIndexArrayRange(added, toBeAdded);
            mesh.RecalculateNormals();
            filter.mesh = mesh;
            // Increment counter
            added += toBeAdded;
            // Add mesh holder to list
            MeshHolders.Add(holder);
        }
    }
}
