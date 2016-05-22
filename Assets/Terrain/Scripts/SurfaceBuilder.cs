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


    // Colors
    [SerializeField]
    private List<Area> Areas;

    [Header("Generation Params")]
    [SerializeField]
    private NoiseParams baseElevation;
    [SerializeField]
    private NoiseParams ridgedMountains;
    [SerializeField]
    private bool useMountainMask;
    [SerializeField]
    private NoiseParams mountainMask;    

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
        float baseComponent = Mathf.Lerp(baseElevation.Low, baseElevation.High, (Noise.Fractal(p.position.x, p.position.z, baseElevation.Frequency, baseElevation.Persistence, baseElevation.Octaves, seed) + 1.0f) / 2.0f);
        float mountainComponent = Mathf.Lerp(ridgedMountains.Low, ridgedMountains.High, (Noise.RidgedFractal(p.position.x, p.position.z, ridgedMountains.Frequency, ridgedMountains.Persistence, ridgedMountains.Octaves, seed) + 1.0f) / 2.0f);
        float mask = useMountainMask ? Noise.CubedFractal(p.position.x, p.position.z, mountainMask.Frequency, mountainMask.Persistence, mountainMask.Octaves, seed) : 1.0f;
        

        p.position.y += (baseComponent + mountainComponent * mask);
    }

    public void SetColor(Triangle tri) {
        float elevation = AvgElevation(tri);
        foreach (Area area in Areas) {
            if (elevation <= area.EndElevation) {
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
