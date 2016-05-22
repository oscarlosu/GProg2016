using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(PlanetBuilder))]
public class PlanetBuilderEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        PlanetBuilder myScript = (PlanetBuilder)target;
        if (GUILayout.Button("Build Planet")) {
            myScript.BuildPlanet();
        }
    }
}
