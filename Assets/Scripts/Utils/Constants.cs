public static class Constants
{
    // ── Grid ─────────────────────────────────────────────────────────────────
    public const int GridColumns = 9;
    public const int GridRows = 16;
    public const float CellSize = 1f;

    // Entry cell: top-center (row 0, column 4)
    public const int GridEntryRow = 0;
    public const int GridEntryColumn = GridColumns / 2; // = 4

    // Exit cell: bottom-center (row 15, column 4)
    public const int GridExitRow = GridRows - 1;        // = 15
    public const int GridExitColumn = GridColumns / 2;  // = 4

    // Errors 
    public const int InvalidCell = -1;

    // ── Towers ────────────────────────────────────────────────────────────────
    public const float TowerTargetingInterval = 0.2f;
    public const string EnemyLayerName = "Enemy";

    // ── Enemies ───────────────────────────────────────────────────────────────
    public const float EnemyExitThreshold = 0.6f;
}
