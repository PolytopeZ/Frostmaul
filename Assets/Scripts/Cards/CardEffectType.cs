public enum CardEffectType
{
    // ── Tower Unlock ──────────────────────────────────────────────────────────
    UnlockTower,            // adds TargetTower to the build menu

    // ── Tower Modifiers (apply to all future placements of TargetTowerType) ──
    TowerDamagePercent,     // value = 0.2 → +20% damage
    TowerRangePercent,
    TowerAttackSpeedPercent,
    TowerCostFlat,          // value = -10 → -10 gold cost

    // ── Global Modifiers ─────────────────────────────────────────────────────
    GlobalDamagePercent,    // value = 0.15 → all towers +15% damage
    GlobalGoldPerKill,      // value = 1 → +1 gold per enemy kill
    BonusLives,             // value = 3 → grant 3 lives immediately

    // ── Maze Card ─────────────────────────────────────────────────────────────
    AddGridRow,             // value = 1 → extend grid height by 1 row

    // ── Relic ─────────────────────────────────────────────────────────────────
    RelicPassive,           // passive effect; logic keyed on CardData identity

    // ── Curse ─────────────────────────────────────────────────────────────────
    EnemySpeedPercent,      // value = 0.3 → enemies 30% faster
    EnemyHpPercent,         // value = 0.5 → enemies 50% more HP

    // ── Boss Skip ─────────────────────────────────────────────────────────────
    SkipNextBoss            // no value needed; sets flag in RunState
}
