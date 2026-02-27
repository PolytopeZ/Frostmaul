using UnityEngine;

[CreateAssetMenu(fileName = "NewTowerData", menuName = "Frostmaul/Tower Data")]
public class TowerData : ScriptableObject
{
    // ── Identity ──────────────────────────────────────────────────────────────
    [SerializeField] private string _displayName;
    [SerializeField][TextArea(2, 4)] private string _description;
    [SerializeField] private Sprite _icon;
    [SerializeField][Range(1, 3)] private int _tier;
    [SerializeField] private TowerType _type;

    // ── Combat ────────────────────────────────────────────────────────────────
    [SerializeField][Min(0f)] private float _damage;
    [SerializeField][Min(0.1f)] private float _range;
    [SerializeField][Min(0.1f)] private float _attackSpeed;

    // ── Economy ───────────────────────────────────────────────────────────────
    [SerializeField][Min(0)] private int _cost;

    // ── Upgrade Tree ──────────────────────────────────────────────────────────
    [SerializeField] private TowerData _upgradePathA; // Tier 1 → 2A, or Tier 2 → 3
    [SerializeField] private TowerData _upgradePathB; // Tier 1 → 2B only; null on Tier 2+

    // ── Runtime Prefab ────────────────────────────────────────────────────────
    [SerializeField] private GameObject _prefab;

    // ── Properties ────────────────────────────────────────────────────────────
    public string DisplayName => _displayName;
    public string Description => _description;
    public Sprite Icon => _icon;
    public int Tier => _tier;
    public TowerType Type => _type;
    public float Damage => _damage;
    public float Range => _range;
    public float AttackSpeed => _attackSpeed;
    public int Cost => _cost;
    public TowerData UpgradePathA => _upgradePathA;
    public TowerData UpgradePathB => _upgradePathB;
    public GameObject Prefab => _prefab;

    public bool CanUpgrade => _upgradePathA != null || _upgradePathB != null;
}
