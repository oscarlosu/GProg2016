using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SurfaceBuilder : MonoBehaviour {
    [SerializeField]
    private Plane PlanePrefab;

    [SerializeField]
    private Plane plane;
    public Plane Plane {
        get {
            if(plane == null) {
                plane = Instantiate(PlanePrefab);
            }
            return plane;
        }
    }

    [SerializeField]
    [Range(0, 6)]
    private int subdivisionSteps = 3;
    [SerializeField]
    [Range(0.5f, 1000)]
    private float sizeX = 10;
    [SerializeField]
    [Range(0.5f, 1000)]
    private float sizeZ = 10;

    [SerializeField]
    [Range(0.0f, 1000000.0f)]
    private float seed = 0;
    [SerializeField]
    private bool useRandomSeed = false;
    [SerializeField]
    private float maxElevation = 1;


    // Colors
    [SerializeField]
    private List<Area> Areas;


    public void BuildSurface() {
        Plane.CreatePlane(sizeX, sizeZ, subdivisionSteps);
        ApplyHeightMap();
        Plane.UpdateMesh();
    }

    private void InitialiseSeed() {
        if(useRandomSeed) {
            seed = Random.Range(0.0f, 1000000.0f); // For some reason, seeds >= 10^8 always produce flat height maps
        }
    }
	
	public void ApplyHeightMap() {
        InitialiseSeed();
        foreach (Triangle tri in Plane.Triangles) {
            ApplyHeightMapToPoint(tri.p1);
            ApplyHeightMapToPoint(tri.p2);
            ApplyHeightMapToPoint(tri.p3);
            SetColor(tri);
        }
    }

    public void ApplyHeightMapToPoint(Point p) {
        p.position.y += (Mathf.PerlinNoise(seed + p.x, seed + p.z) * maxElevation);
    }

    public void SetColor(Triangle tri) {
        float elevation = AvgElevation(tri);
        foreach (Area area in Areas) {
            if (elevation <= area.endElevation) {
                tri.color = area.GetColor(tri);
                break;
            }
        }
    }

    private float AvgElevation(Triangle tri) {
        float elevation = tri.p1.position.y + tri.p2.position.y + tri.p3.position.y;
        elevation /= 3.0f;
        return elevation;
    }
}
