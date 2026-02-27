using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyPool _pool;
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private EnemyData[] _sequence;
    [SerializeField] private float _spawnInterval = 2f;

    private int _nextIndex;
    private float _timer;

    private void OnEnable()
    {
        _nextIndex = 0;
        _timer = 0f;
    }

    private void Update()
    {
        if (_sequence == null || _sequence.Length == 0) return;

        _timer -= Time.deltaTime;
        if (_timer > 0f) return;

        _timer = _spawnInterval;
        SpawnNext();
    }

    private void SpawnNext()
    {
        EnemyData data = _sequence[_nextIndex % _sequence.Length];
        _nextIndex++;

        EnemyBase enemy = _pool.Get(data);
        if (enemy == null) return;

        enemy.transform.position = _gridManager.CellToWorld(
            new Vector2Int(Constants.GridEntryColumn, Constants.GridEntryRow));
    }
}
