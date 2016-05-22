using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(SurfaceBuilder))]
public class SurfaceBuilderEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        SurfaceBuilder myScript = (SurfaceBuilder)target;
        if (GUILayout.Button("Build Surface")) {
            myScript.BuildSurface();
        }
    }
}
