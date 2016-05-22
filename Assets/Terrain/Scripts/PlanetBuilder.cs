using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlanetBuilder : MonoBehaviour {
    [SerializeField]
    private IcoSphere IcoSpherePrefab;

    [SerializeField]
    private IcoSphere land;
    public IcoSphere Land {
        get {
            if(land == null) {
                land = Instantiate(IcoSpherePrefab);
            }
            return land;
        }
    }

    [SerializeField]
    private IcoSphere water;
    public IcoSphere Water {
        get {
            if (water == null) {
                water = Instantiate(IcoSpherePrefab);
            }
            return water;
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
    [Header("Land Generation Params")]
    [SerializeField]
    private List<Area> LandAreas;
    [SerializeField]
    private NoiseParams baseElevation;
    [SerializeField]
    private NoiseParams ridgedMountains;
    [SerializeField]
    private bool useMountainMask;
    [SerializeField]
    private NoiseParams mountainMask;

    [Header("Water Generation Params")]
    [SerializeField]
    private Area WaterArea;
    [SerializeField]
    private NoiseParams waterParams;
    [SerializeField]
    private bool useWaterMask;
    [SerializeField]
    private NoiseParams waterMask;



    public void BuildPlanet() {
        Land.CreateIcosphere(radius, subdivisionSteps);
        Water.CreateIcosphere(radius, subdivisionSteps);
        ApplyHeightMap();
        Land.UpdateMesh();
        Water.UpdateMesh();
    }

    private void InitialiseSeed() {
        if(useRandomSeed) {
            seed = Random.Range(0.0f, 1000000.0f); // For some reason, seeds >= 10^8 always produce flat height maps
        }
    }
	
	public void ApplyHeightMap() {
        InitialiseSeed();
        // Land
        foreach (Triangle tri in Land.Triangles) {
            ApplyHeightMapToLandPoint(tri.p1);
            ApplyHeightMapToLandPoint(tri.p2);
            ApplyHeightMapToLandPoint(tri.p3);
            SetLandColor(tri);
        }
        // Water
        foreach (Triangle tri in Water.Triangles) {
            ApplyHeightMapToWaterPoint(tri.p1);
            ApplyHeightMapToWaterPoint(tri.p2);
            ApplyHeightMapToWaterPoint(tri.p3);
            SetWaterColor(tri);
        }
    }

    public void ApplyHeightMapToLandPoint(Point p) {
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

    public void ApplyHeightMapToWaterPoint(Point p) {
        float theta = Mathf.Acos(p.position.z / radius) * Mathf.Rad2Deg;
        float phi = Mathf.Atan2(p.position.y, p.position.x) * Mathf.Rad2Deg;

        float waterComponent = Mathf.Lerp(waterParams.Low, waterParams.High, (Noise.RidgedFractal(theta, phi, waterParams.Frequency, waterParams.Persistence, waterParams.Octaves, seed) + 1.0f) / 2.0f);
        float mask = useWaterMask ?
            Mathf.Clamp(Mathf.Lerp(waterMask.Low, waterMask.High,
                                   (Noise.CubedFractal(theta, phi, waterMask.Frequency, waterMask.Persistence, waterMask.Octaves, seed) + 1.0f) / 2.0f),
                                   0.0f, 1.0f) :
            1.0f;


        p.position += (waterComponent * mask) * p.position.normalized;
    }

    public void SetLandColor(Triangle tri) {
        float elevation = AvgElevation(tri);
        foreach (Area area in LandAreas) {
            if (elevation <= area.EndElevation) {
                tri.color = area.GetColor(tri);
                break;
            }
        }
    }

    public void SetWaterColor(Triangle tri) {
        tri.color = WaterArea.GetColor(tri);
    }

    private float AvgElevation(Triangle tri) {
        float elevation = (tri.p1.position.magnitude - radius) +
                          (tri.p2.position.magnitude - radius) + 
                          (tri.p3.position.magnitude - radius);
        elevation /= 3.0f;
        return elevation;
    }
}
