using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TextController), true)]
public class TextControllerEditor : Editor
{ 
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        TextController thisScript = (TextController)target;
        if (Application.isPlaying)
            if (GUILayout.Button("Debug Start Dialogue"))
                thisScript.StartDialogue(thisScript.textData);
    }
}
