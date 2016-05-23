using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SurfaceBuilder : MonoBehaviour {
    [SerializeField]
    private Plane PlanePrefab;

    [SerializeField]
    private Plane land;
    public Plane Land {
        get {
            if(land == null) {
                land = Instantiate(PlanePrefab);
            }
            return land;
        }
    }

    [SerializeField]
    private Plane water;
    public Plane Water {
        get {
            if (water == null) {
                water = Instantiate(PlanePrefab);
            }
            return water;
        }
    }

    [SerializeField]
    [Range(0, 10)]
    private int subdivisionSteps = 3;
    [SerializeField]
    [Range(0.5f, 10000)]
    private float sizeX = 10;
    [SerializeField]
    [Range(0.5f, 10000)]
    private float sizeZ = 10;

    [SerializeField]
    [Range(0.0f, 1000000.0f)]
    private float seed = 0;
    [SerializeField]
    private bool useRandomSeed = false;

    //[SerializeField]
    //[Tooltip("Water mesh will be created using the parameters in the first Area of the Area List")]
    //private bool createWater = true;

    [Header("Land Generation Params")]
    [SerializeField]
    private Material LandMaterial;
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
    private Material WaterMaterial;
    [SerializeField]
    private bool CreateWater = true;
    [SerializeField]
    private Area WaterArea;
    [SerializeField]
    private NoiseParams waterParams;
    [SerializeField]
    private bool useWaterMask;
    [SerializeField]
    private NoiseParams waterMask;

    public void BuildSurface() {
        Land.CreatePlane(sizeX, sizeZ, subdivisionSteps);
        if (CreateWater) {
            Water.CreatePlane(sizeX, sizeZ, subdivisionSteps);
        } else {
            Water.ClearMesh();
        }
        ApplyHeightMap();
        Land.UpdateMesh(LandMaterial);
        if (CreateWater) {
            Water.UpdateMesh(WaterMaterial);
        }
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
        if (CreateWater) {
            // Water
            foreach (Triangle tri in Water.Triangles) {
                ApplyHeightMapToWaterPoint(tri.p1);
                ApplyHeightMapToWaterPoint(tri.p2);
                ApplyHeightMapToWaterPoint(tri.p3);
                SetWaterColor(tri);
            }
        }
    }

    public void ApplyHeightMapToLandPoint(Point p) {
        float baseComponent = Mathf.Lerp(baseElevation.Low, baseElevation.High, (Noise.Fractal(p.position.x, p.position.z, baseElevation.Frequency, baseElevation.Persistence, baseElevation.Octaves, seed) + 1.0f) / 2.0f);
        float mountainComponent = Mathf.Lerp(ridgedMountains.Low, ridgedMountains.High, (Noise.RidgedFractal(p.position.x, p.position.z, ridgedMountains.Frequency, ridgedMountains.Persistence, ridgedMountains.Octaves, seed) + 1.0f) / 2.0f);
        float mask = useMountainMask ?
            Mathf.Clamp(Mathf.Lerp(mountainMask.Low, mountainMask.High,
                                   (Noise.CubedFractal(p.position.x, p.position.z, mountainMask.Frequency, mountainMask.Persistence, mountainMask.Octaves, seed) + 1.0f) / 2.0f),
                                   0.0f, 1.0f) :
            1.0f;


        p.position.y += (baseComponent + mountainComponent * mask);
    }

    public void ApplyHeightMapToWaterPoint(Point p) {
        float waterComponent = Mathf.Lerp(waterParams.Low, waterParams.High, (Noise.RidgedFractal(p.position.x, p.position.z, waterParams.Frequency, waterParams.Persistence, waterParams.Octaves, seed) + 1.0f) / 2.0f);
        float mask = useWaterMask ?
            Mathf.Clamp(Mathf.Lerp(waterMask.Low, waterMask.High,
                                   (Noise.CubedFractal(p.position.x, p.position.z, waterMask.Frequency, waterMask.Persistence, waterMask.Octaves, seed) + 1.0f) / 2.0f),
                                   0.0f, 1.0f) :
            1.0f;


        p.position.y += (waterComponent * mask);
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
        float elevation = tri.p1.position.y + tri.p2.position.y + tri.p3.position.y;
        elevation /= 3.0f;
        return elevation;
    }
}
