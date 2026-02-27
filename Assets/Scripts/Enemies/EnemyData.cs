using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Frostmaul/Enemy Data")]
public class EnemyData : ScriptableObject
{
    // ── Identity ──────────────────────────────────────────────────────────────
    [SerializeField] private string _displayName;
    [SerializeField] private Sprite _icon;

    // ── Stats ─────────────────────────────────────────────────────────────────
    [SerializeField][Min(1f)] private float _maxHp;
    [SerializeField][Min(0.1f)] private float _speed;
    [SerializeField][Min(0f)] private float _armor;
    [SerializeField][Min(0)] private int _rewardGold;

    // ── Behavior ──────────────────────────────────────────────────────────────
    [SerializeField] private bool _isFlyer;

    // ── Runtime Prefab ────────────────────────────────────────────────────────
    [SerializeField] private GameObject _prefab;

    // ── Properties ────────────────────────────────────────────────────────────
    public string DisplayName => _displayName;
    public Sprite Icon => _icon;
    public float MaxHp => _maxHp;
    public float Speed => _speed;
    public float Armor => _armor;
    public int RewardGold => _rewardGold;
    public bool IsFlyer => _isFlyer;
    public GameObject Prefab => _prefab;
}
