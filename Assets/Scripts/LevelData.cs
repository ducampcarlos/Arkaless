using UnityEngine;

[CreateAssetMenu(fileName = "NewLevel", menuName = "Arkaless/Level Data")]
public class LevelData : ScriptableObject
{
    [Tooltip("Number of columns in the level grid")]
    public int width = 10;
    [Tooltip("Number of rows in the level grid")]
    public int height = 5;

    [Tooltip("Flattened array of durability levels; length = width * height")]
    public int[] cellDurabilities;

    public void OnValidate()
    {
        // Asegurarnos de que el array tenga el tamaño correcto
        if (cellDurabilities == null || cellDurabilities.Length != width * height)
        {
            cellDurabilities = new int[width * height];
        }
    }
}
