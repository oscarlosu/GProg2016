using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(IcoSphere))]
public class IcoSphereEditor : Editor {

    private SerializedProperty radius;

    void OnEnable() {
        radius = serializedObject.FindProperty("radius");
    }
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        IcoSphere myScript = (IcoSphere)target;
        if (GUILayout.Button("Build IcoSphere")) {
            myScript.CreateIcosphere();
            myScript.UpdateMesh();
        }
        serializedObject.Update();
        radius.floatValue = Mathf.Max(0.5f, radius.floatValue);
        serializedObject.ApplyModifiedProperties();
    }
}
