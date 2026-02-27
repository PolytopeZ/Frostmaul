using System.Collections.Generic;

public class RunState
{
    // ── Unlocked Towers ───────────────────────────────────────────────────────
    public readonly List<TowerData> UnlockedTowers = new List<TowerData>();

    // ── Global Modifiers ──────────────────────────────────────────────────────
    public float GlobalDamageMultiplier = 1f;
    public int ExtraGoldPerKill = 0;

    // ── Per-Type Tower Modifiers ──────────────────────────────────────────────
    public readonly Dictionary<TowerType, TowerModifiers> TowerMods
        = new Dictionary<TowerType, TowerModifiers>();

    // ── Enemy Modifiers (Curses) ──────────────────────────────────────────────
    public float EnemySpeedMultiplier = 1f;
    public float EnemyHpMultiplier = 1f;

    // ── Flags ─────────────────────────────────────────────────────────────────
    public bool SkipNextBoss = false;
    public int RerollsRemaining = Constants.CardRerollsPerDraw;

    // ── Maze ──────────────────────────────────────────────────────────────────
    public int BonusGridRows = 0;

    // ── Relics ────────────────────────────────────────────────────────────────
    public readonly List<CardData> ActiveRelics = new List<CardData>();

    // ── Helpers ───────────────────────────────────────────────────────────────
    public TowerModifiers GetTowerMods(TowerType type)
    {
        if (!TowerMods.TryGetValue(type, out TowerModifiers mods))
        {
            mods = new TowerModifiers();
            TowerMods[type] = mods;
        }
        return mods;
    }
}
