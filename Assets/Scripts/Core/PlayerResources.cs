using UnityEngine;

public class PlayerResources : MonoBehaviour
{
    [SerializeField] private int _startingGold = 100;
    [SerializeField] private int _startingLives = 20;

    private int _gold;
    private int _killCount;
    private int _lives;

    public int Gold => _gold;
    public int KillCount => _killCount;
    public int Lives => _lives;

    public static event System.Action<int> OnGoldChanged;
    public static event System.Action<int> OnKillCountChanged;
    public static event System.Action<int> OnLivesChanged;

    private void Awake()
    {
        _gold = _startingGold;
        _lives = _startingLives;
    }

    private void OnEnable()
    {
        EnemyBase.OnEnemyKilled += HandleEnemyKilled;
        EnemyBase.OnEnemyReachedExit += HandleEnemyEscaped;
    }

    private void OnDisable()
    {
        EnemyBase.OnEnemyKilled -= HandleEnemyKilled;
        EnemyBase.OnEnemyReachedExit -= HandleEnemyEscaped;
    }

    private void HandleEnemyKilled(EnemyBase enemy)
    {
        int bonus = RunManager.Current?.ExtraGoldPerKill ?? 0;
        _gold += enemy.Data.RewardGold + bonus;
        _killCount++;

        OnGoldChanged?.Invoke(_gold);
        OnKillCountChanged?.Invoke(_killCount);
    }

    private void HandleEnemyEscaped(EnemyBase enemy)
    {
        Debug.Log($"Escaped! Lives: {_lives}");
        _lives = Mathf.Max(0, _lives - 1);
        OnLivesChanged?.Invoke(_lives);
    }

    // Called by future shop/upgrade systems
    public bool TrySpend(int amount)
    {
        if (_gold < amount) return false;
        _gold -= amount;
        OnGoldChanged?.Invoke(_gold);
        return true;
    }

    public void AddGold(int amount)
    {
        _gold += amount;
        OnGoldChanged?.Invoke(_gold);
    }

    public void AddLives(int amount)
    {
        _lives += amount;
        OnLivesChanged?.Invoke(_lives);
    }
}
