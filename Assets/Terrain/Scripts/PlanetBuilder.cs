using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlanetBuilder : MonoBehaviour {
    [SerializeField]
    private IcoSphere IcoSpherePrefab;

    [SerializeField]
    private IcoSphere icosphere;
    public IcoSphere IcoSphere {
        get {
            if(icosphere == null) {
                icosphere = Instantiate(IcoSpherePrefab);
            }
            return icosphere;
        }
    }

    [SerializeField]
    [Range(0, 5)]
    private int subdivisionSteps = 3;
    [SerializeField]
    [Range(0.5f, 1000)]
    private float radius = 10;

    [SerializeField]
    [Range(0.0f, 1000000.0f)]
    private float seed = 0;
    [SerializeField]
    private bool useRandomSeed = false;

    // Colors
    [SerializeField]
    private List<Area> areas;
    public List<Area> Areas {
        get {
            return areas;
        }
    }

    [Header("Generation Params")]
    [SerializeField]
    private NoiseParams baseElevation;
    [SerializeField]
    private NoiseParams ridgedMountains;
    [SerializeField]
    private bool useMountainMask;
    [SerializeField]
    private NoiseParams mountainMask;



    public void BuildPlanet() {
        IcoSphere.CreateIcosphere(radius, subdivisionSteps);
        ApplyHeightMap();
        IcoSphere.UpdateMesh();
    }

    private void InitialiseSeed() {
        if(useRandomSeed) {
            seed = Random.Range(0.0f, 1000000.0f); // For some reason, seeds >= 10^8 always produce flat height maps
        }
    }
	
	public void ApplyHeightMap() {
        InitialiseSeed();
        foreach (Triangle tri in IcoSphere.Triangles) {
            ApplyHeightMapToPoint(tri.p1);
            ApplyHeightMapToPoint(tri.p2);
            ApplyHeightMapToPoint(tri.p3);
            SetColor(tri);
        }
    }

    public void ApplyHeightMapToPoint(Point p) {
        float theta = Mathf.Acos(p.position.z / radius) * Mathf.Rad2Deg;
        float phi = Mathf.Atan2(p.position.y, p.position.x) * Mathf.Rad2Deg;

        float baseComponent = Mathf.Lerp(baseElevation.Low, baseElevation.High, (Noise.Fractal(theta, phi, baseElevation.Frequency, baseElevation.Persistence, baseElevation.Octaves, seed) + 1.0f) / 2.0f);
        float mountainComponent = Mathf.Lerp(ridgedMountains.Low, ridgedMountains.High, (Noise.RidgedFractal(theta, phi, ridgedMountains.Frequency, ridgedMountains.Persistence, ridgedMountains.Octaves, seed) + 1.0f) / 2.0f);
        float mask = useMountainMask ? 
            Mathf.Clamp(Mathf.Lerp(mountainMask.Low, mountainMask.High, 
                                   (Noise.CubedFractal(theta, phi, mountainMask.Frequency, mountainMask.Persistence, mountainMask.Octaves, seed) + 1.0f) / 2.0f), 
                                   0.0f, 1.0f) : 
            1.0f;


        p.position += (baseComponent + mountainComponent * mask) * p.position.normalized;
    }

    public void SetColor(Triangle tri) {
        float elevation = AvgElevation(tri);
        foreach (Area area in areas) {
            if (elevation <= area.EndElevation) {
                tri.color = area.GetColor(tri);
                break;
            }
        }
    }

    private float AvgElevation(Triangle tri) {
        float elevation = (tri.p1.position.magnitude - radius) +
                          (tri.p2.position.magnitude - radius) + 
                          (tri.p3.position.magnitude - radius);
        elevation /= 3.0f;
        return elevation;
    }
}
