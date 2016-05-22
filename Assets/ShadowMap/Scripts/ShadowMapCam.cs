using UnityEngine;
using System.Collections;

public class ShadowMapCam : MonoBehaviour {
	[SerializeField]
	private Shader shader;
	[SerializeField]
	[HideInInspector]
	private Color col;
	public Color ShadowColor {
		get {
			return col;
		}
		set {
			col = value;
			UpdateShadowColor();
		}
	}
	[SerializeField]
	private RenderTexture shadowMap;

	Camera cam;

	// Use this for initialization
	void Start () {
		cam = GetComponent<Camera>();
		UpdateShadowColor();
		cam.SetReplacementShader(shader, "");
	}

	private void UpdateShadowColor() {
		Shader.SetGlobalVector("_ShadowColor", new Vector4(ShadowColor.r, ShadowColor.g, ShadowColor.b, ShadowColor.a));
		Shader.SetGlobalTexture("_ShadowMap", shadowMap);
	}
}