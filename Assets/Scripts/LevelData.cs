using UnityEngine;

[CreateAssetMenu(fileName = "NewLevel", menuName = "Arkanoid/Level Data")]
public class LevelData : ScriptableObject
{
    [Tooltip("Number of columns in the level grid")]
    public int width = 10;
    [Tooltip("Number of rows in the level grid")]
    public int height = 5;

    [Tooltip("Flattened array of durability levels; length = width * height")]
    public int[] cellDurabilities;

    [Tooltip("Colors for each durability level; index 0 = durability 1")]
    public Color[] durabilityColors = new Color[4];

    public void OnValidate()
    {
        // Ensure cellDurabilities has exactly width*height entries
        int size = width * height;
        if (cellDurabilities == null || cellDurabilities.Length != size)
            cellDurabilities = new int[size];

        // Ensure durabilityColors has at least one entry
        if (durabilityColors == null || durabilityColors.Length == 0)
            durabilityColors = new Color[] { Color.red, Color.yellow, Color.green, Color.cyan };
    }
}
