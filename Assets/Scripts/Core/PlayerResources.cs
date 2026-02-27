using UnityEngine;

public class PlayerResources : MonoBehaviour
{
    [SerializeField] private int _startingGold = 100;

    private int _gold;
    private int _killCount;

    public int Gold => _gold;
    public int KillCount => _killCount;

    public static event System.Action<int> OnGoldChanged;
    public static event System.Action<int> OnKillCountChanged;

    private void Awake()
    {
        _gold = _startingGold;
    }

    private void OnEnable()
    {
        EnemyBase.OnEnemyKilled += HandleEnemyKilled;
    }

    private void OnDisable()
    {
        EnemyBase.OnEnemyKilled -= HandleEnemyKilled;
    }

    private void HandleEnemyKilled(EnemyBase enemy)
    {
        _gold += enemy.Data.RewardGold;
        _killCount++;

        OnGoldChanged?.Invoke(_gold);
        OnKillCountChanged?.Invoke(_killCount);
    }

    // Called by future shop/upgrade systems
    public bool TrySpend(int amount)
    {
        if (_gold < amount) return false;
        _gold -= amount;
        OnGoldChanged?.Invoke(_gold);
        return true;
    }
}
