using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelManager))]
public class LevelManagement : Editor
{
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        LevelManager manager = (LevelManager)target;

        GUILayout.Label("");

        if (GUILayout.Button("Reset levels unlocked"))
        {
            Undo.RecordObject(target, "Reset levels unlocked");
            manager.ResetLevelsUnlocked();
            EditorUtility.SetDirty(target);
        }

        if (GUILayout.Button("Increase levels unlocked"))
        {
            Undo.RecordObject(target, "Increase levels unlocked");
            manager.IncreaseLevelsUnlocked();
            EditorUtility.SetDirty(target);
        }

        if (GUILayout.Button("Decrease levels unlocked"))
        {
            Undo.RecordObject(target, "Decrease levels unlocked");
            manager.DecreaseLevelsUnlocked();
            EditorUtility.SetDirty(target);
        }
    }
}
