#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class LevelBatchGeneratorWindow : EditorWindow
{
    int numberOfLevels = 10;
    int maxWidth = 15;
    int maxHeight = 6;
    int maxDurability = 4;
    float baseEmptyCellChance = 0.3f;
    string targetFolder = "Assets/Scripts/Levels";
    const int minBlocks = 15;

    enum PatternType { Random, StairUp, StairDown, Border, Checkerboard, SolidSquare, Cross, Diagonal }
    PatternType[] patterns = (PatternType[])Enum.GetValues(typeof(PatternType));

    [MenuItem("Window/Arkanoid Level Batch Generator")]
    static void OpenWindow() => GetWindow<LevelBatchGeneratorWindow>("Batch Level Gen");

    void OnGUI()
    {
        GUILayout.Label("Batch Level Generator", EditorStyles.boldLabel);
        numberOfLevels = EditorGUILayout.IntField("Number of Levels", numberOfLevels);
        maxWidth = EditorGUILayout.IntSlider("Max Width", maxWidth, 1, 15);
        maxHeight = EditorGUILayout.IntSlider("Max Height", maxHeight, 1, 6);
        maxDurability = EditorGUILayout.IntSlider("Max Durability", maxDurability, 1, 10);
        baseEmptyCellChance = EditorGUILayout.Slider("Base Empty Cell Chance", baseEmptyCellChance, 0f, 1f);
        EditorGUILayout.Space();
        targetFolder = EditorGUILayout.TextField("Output Folder", targetFolder);
        if (GUILayout.Button("Generate Levels")) GenerateLevels();
    }

    void GenerateLevels()
    {
        if (!AssetDatabase.IsValidFolder(targetFolder))
        {
            string fullPath = Path.Combine(Application.dataPath, targetFolder.Substring("Assets/".Length));
            Directory.CreateDirectory(fullPath);
            AssetDatabase.Refresh();
        }

        int previousTotalDurability = 0;

        for (int i = 1; i <= numberOfLevels; i++)
        {
            LevelData lvl = ScriptableObject.CreateInstance<LevelData>();
            float difficulty = numberOfLevels > 1 ? (float)(i - 1) / (numberOfLevels - 1) : 0f;

            PatternType pattern = patterns[(i - 1) % patterns.Length];

            int w, h;
            do
            {
                w = UnityEngine.Random.Range(1, maxWidth + 1);
                h = UnityEngine.Random.Range(1, maxHeight + 1);
            }
            while (w * h < minBlocks);
            lvl.width = w;
            lvl.height = h;
            lvl.OnValidate();

            float emptyChance = Mathf.Lerp(baseEmptyCellChance, 0.1f, difficulty);
            int minDur = 1;
            int maxDur = Mathf.Max(1, Mathf.RoundToInt(Mathf.Lerp(1, maxDurability, difficulty)));

            for (int y = 0; y < lvl.height; y++)
                for (int x = 0; x < lvl.width; x++)
                {
                    int idx = y * lvl.width + x;
                    bool place = pattern switch
                    {
                        PatternType.Random => UnityEngine.Random.value >= emptyChance,
                        PatternType.StairUp => x <= y,
                        PatternType.StairDown => (lvl.height - 1 - y) <= x,
                        PatternType.Border => x == 0 || y == 0 || x == lvl.width - 1 || y == lvl.height - 1,
                        PatternType.Checkerboard => (x + y) % 2 == 0,
                        PatternType.SolidSquare => true,
                        PatternType.Cross => x == lvl.width / 2 || y == lvl.height / 2,
                        PatternType.Diagonal => x == y,
                        _ => false
                    };
                    lvl.cellDurabilities[idx] = place
                        ? UnityEngine.Random.Range(minDur, maxDur + 1)
                        : 0;
                }

            int placedCount = 0;
            List<int> zeroIndices = new List<int>();
            for (int idx = 0; idx < lvl.cellDurabilities.Length; idx++)
            {
                if (lvl.cellDurabilities[idx] > 0) placedCount++;
                else zeroIndices.Add(idx);
            }

            if (placedCount < minBlocks)
            {
                int needed = minBlocks - placedCount;
                for (int k = 0; k < needed && zeroIndices.Count > 0; k++)
                {
                    int r = UnityEngine.Random.Range(0, zeroIndices.Count);
                    int idx = zeroIndices[r];
                    lvl.cellDurabilities[idx] = UnityEngine.Random.Range(minDur, maxDur + 1);
                    zeroIndices.RemoveAt(r);
                }
            }

            // --- Asegurar suma creciente de durabilidades ---
            // Calculamos la suma actual:
            int totalDurability = lvl.cellDurabilities.Sum();

            // Lista de índices cuya durabilidad aún puede aumentar
            List<int> increasable = Enumerable.Range(0, lvl.cellDurabilities.Length)
                .Where(idx => lvl.cellDurabilities[idx] > 0 && lvl.cellDurabilities[idx] < maxDur)
                .ToList();

            // Mientras no supere la anterior, incrementamos aleatoriamente
            while (totalDurability <= previousTotalDurability && increasable.Count > 0)
            {
                int randIndex = increasable[UnityEngine.Random.Range(0, increasable.Count)];
                lvl.cellDurabilities[randIndex]++;
                totalDurability++;
                if (lvl.cellDurabilities[randIndex] >= maxDur)
                    increasable.Remove(randIndex);
            }

            previousTotalDurability = totalDurability;
            // --------------------------------------------------

            string assetPath = $"{targetFolder}/Level_{i:000}.asset";
            AssetDatabase.CreateAsset(lvl, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Batch Generation",
            $"{numberOfLevels} levels generated in\n{targetFolder}", "OK");
    }
}
#endif
