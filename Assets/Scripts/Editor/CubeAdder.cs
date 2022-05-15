/*using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Cube))]
public class CubeAdder : Editor
{
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        Cube cube = (Cube)target;

        GUILayout.Label("");

        if (GUILayout.Button("Add this cube to Path"))
        {
            Undo.RecordObject(cube.gameManager, "Added cube to path");
            cube.AddThisToPath();
            EditorUtility.SetDirty(cube.gameManager);
        }

        if (GUILayout.Button("Remove this cube from Path"))
        {
            Undo.RecordObject(cube.gameManager, "Removed cube from path");
            cube.RemoveThisFromPath();
            EditorUtility.SetDirty(cube.gameManager);
        }
    }
}*/
