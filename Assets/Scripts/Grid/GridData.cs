using UnityEngine;

[CreateAssetMenu(fileName = "GridData", menuName = "Frostmaul/Grid/GridData")]
public class GridData : ScriptableObject
{
    [Header("Dimensions")]
    [SerializeField] private int _columns = 9;
    [SerializeField] private int _rows = 16;

    [Header("Designer-Blocked Cells")]
    [Tooltip("Cells that are permanently Blocked regardless of game state.")]
    [SerializeField] private Vector2Int[] _blockedCells = new Vector2Int[0];

    public int Columns => _columns;
    public int Rows => _rows;
    public Vector2Int[] BlockedCells => _blockedCells;
}
