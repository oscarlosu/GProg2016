using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(ShadowMapCam))]
public class ShadowMapCamEditor : Editor {
	public override void OnInspectorGUI() {
		ShadowMapCam t = (ShadowMapCam)target;
		t.ShadowColor = EditorGUILayout.ColorField("Shadow Color", t.ShadowColor);
		DrawDefaultInspector();
	}

}
