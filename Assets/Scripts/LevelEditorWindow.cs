#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class LevelEditorWindow : EditorWindow
{
    LevelData levelData;
    Vector2 scroll;

    [MenuItem("Window/Arkanoid Level Editor")]
    static void OpenWindow() => GetWindow<LevelEditorWindow>("Level Editor");

    void OnGUI()
    {
        // 1) Field to select the LevelData asset
        levelData = (LevelData)EditorGUILayout.ObjectField("Level Data", levelData, typeof(LevelData), false);
        if (levelData == null) return;

        // Create a SerializedObject to handle the color array
        SerializedObject so = new SerializedObject(levelData);
        so.Update();

        // 2) Edit the grid dimensions
        EditorGUILayout.Space();
        levelData.width = EditorGUILayout.IntSlider(new GUIContent("Grid Width"), levelData.width, 1, 20);
        levelData.height = EditorGUILayout.IntSlider(new GUIContent("Grid Height"), levelData.height, 1, 10);
        levelData.OnValidate();

        // 3) Palette Editor: lets you define each color
        EditorGUILayout.LabelField("Block Colors (by Durability)", EditorStyles.boldLabel);
        SerializedProperty colorsProp = so.FindProperty("durabilityColors");
        EditorGUILayout.PropertyField(colorsProp, new GUIContent("Durability Colors"), true);
        so.ApplyModifiedProperties();

        // 4) Draw the interactive grid
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Paint Blocks (click to cycle 0→N)", EditorStyles.boldLabel);

        Color[] palette = levelData.durabilityColors;
        int maxDur = palette.Length;
        scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(300));

        for (int y = levelData.height - 1; y >= 0; y--)
        {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < levelData.width; x++)
            {
                int idx = y * levelData.width + x;
                int val = levelData.cellDurabilities[idx];

                // Set the button color: gray for 0 (empty), otherwise palette[val-1]
                GUI.backgroundColor = (val >= 1 && val <= maxDur)
                    ? palette[val - 1]
                    : Color.gray;

                // Draw button labeled with current durability
                if (GUILayout.Button(val.ToString(), GUILayout.Width(30), GUILayout.Height(30)))
                {
                    // Cycle durability: 0→1→2→…→maxDur→0
                    levelData.cellDurabilities[idx] = (val + 1) % (maxDur + 1);
                }
                GUI.backgroundColor = Color.white;
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();

        // 5) Mark asset dirty if changed
        if (GUI.changed)
            EditorUtility.SetDirty(levelData);
    }
}
#endif
