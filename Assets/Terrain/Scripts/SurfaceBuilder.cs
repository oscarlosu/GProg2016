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
    [Range(0, 7)]
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
        foreach (Point p in Plane.Points) {
            p.position.y += (Mathf.PerlinNoise(seed + p.x, seed + p.z) * maxElevation);
            SetColor(p);
        }
    }

    public void SetColor(Point p) {
        foreach (Area area in Areas) {
            if(p.y <= area.endElevation) {
                p.color = area.GetColor(p);
                break;
            }
        }
    }
}
