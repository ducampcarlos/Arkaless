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
        levelData = (LevelData)EditorGUILayout.ObjectField("Level Data", levelData, typeof(LevelData), false);
        if (levelData == null) return;

        // Ajustes de grilla
        levelData.width = EditorGUILayout.IntSlider(new GUIContent("Width"), levelData.width, 1, 20);
        levelData.height = EditorGUILayout.IntSlider(new GUIContent("Height"), levelData.height, 1, 10);
        levelData.OnValidate();

        // Colores de los niveles de durabilidad
        Color[] durabilityColors = { Color.red, Color.yellow, Color.green, Color.cyan, Color.magenta };

        // Scroll para la grilla
        scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(300));
        for (int y = 0; y < levelData.height; y++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < levelData.width; x++)
            {
                int idx = y * levelData.width + x;
                int val = levelData.cellDurabilities[idx];
                // botón que muestra color según durabilidad
                GUI.backgroundColor = (val >= 1 && val <= durabilityColors.Length)
                    ? durabilityColors[val - 1]
                    : Color.gray;
                if (GUILayout.Button(val.ToString(), GUILayout.Width(30), GUILayout.Height(30)))
                {
                    // clic para ciclar durabilidad
                    levelData.cellDurabilities[idx] = (val + 1) % (durabilityColors.Length + 1);
                }
                GUI.backgroundColor = Color.white;
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(levelData);
        }
    }
}
#endif
