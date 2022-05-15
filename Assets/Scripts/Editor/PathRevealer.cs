using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameManager))]
public class PathRevealer : Editor
{
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        GameManager gm = (GameManager)target;

        GUILayout.Label("");

        if (GUILayout.Button("Show level path"))
        {
            gm.ShowLevelPath();
        }

        if (GUILayout.Button("Hide level path"))
        {
            gm.HideLevelPath();
        }

        if (GUILayout.Button("Reset all cubes color"))
        {
            gm.ResetAllCubesColor();
        }

        if (GUILayout.Button("Reset path"))
        {
            Undo.RecordObject(target, "Reset path");
            gm.ResetPath();
            EditorUtility.SetDirty(target);
        }
    }
}
