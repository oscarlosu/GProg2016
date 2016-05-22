using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Plane))]
public class PlaneEditor : Editor {

    private SerializedProperty sizeX;
    private SerializedProperty sizeZ;

    void OnEnable() {
        sizeX = serializedObject.FindProperty("sizeX");
        sizeZ = serializedObject.FindProperty("sizeZ");
    }
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        Plane myScript = (Plane)target;
        if (GUILayout.Button("Build Plane")) {
            myScript.CreatePlane();
            myScript.UpdateMesh();
        }
        serializedObject.Update();
        sizeX.floatValue = Mathf.Max(0.5f, sizeX.floatValue);
        sizeZ.floatValue = Mathf.Max(0.5f, sizeZ.floatValue);
        serializedObject.ApplyModifiedProperties();
    }
}
