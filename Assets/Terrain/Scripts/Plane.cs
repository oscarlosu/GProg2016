using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class Plane : MonoBehaviour {


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

    public TriangleList Triangles { get; set; }

    [SerializeField]
    [Range(0, 10)]
    private int subdivisionSteps = 0;
    [SerializeField]
    private float sizeX = 1;
    [SerializeField]
    private float sizeZ = 1;
    [SerializeField]
    private Material defaultMat;

    public void ClearMesh() {
        for(int i = MeshHolders.Count - 1; i >= 0; --i) {
            DestroyImmediate(MeshHolders[i]);            
        }
        meshHolders.Clear();
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
        UpdateMesh(defaultMat);
    }

    public void UpdateMesh(Material mat) {
        ClearMesh();
        // Figure out how many meshes will be necessary
        int nMeshes = (int)Mathf.Ceil((float)(Triangles.Count * 3.0f) / (float)maxTrianglesInMesh);
        // Initialise counter
        int added = 0;

        while(MeshHolders.Count < nMeshes) {
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

