using System;
using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] private EnemyPool _enemyPool;
    [SerializeField] private GridManager _gridManager;

    private int _remainingEnemies;
    private bool _spawningDone;
    private bool _waveActive;

    public static event Action OnWaveComplete;

    private void OnEnable()
    {
        EnemyBase.OnEnemyKilled += HandleEnemyRemoved;
        EnemyBase.OnEnemyReachedExit += HandleEnemyRemoved;
    }

    private void OnDisable()
    {
        EnemyBase.OnEnemyKilled -= HandleEnemyRemoved;
        EnemyBase.OnEnemyReachedExit -= HandleEnemyRemoved;
    }

    public void StartWave(WaveData wave)
    {
        _remainingEnemies = 0;
        _spawningDone = false;
        _waveActive = true;
        StartCoroutine(SpawnRoutine(wave));
    }

    private IEnumerator SpawnRoutine(WaveData wave)
    {
        foreach (WaveEntry entry in wave.Entries)
        {
            for (int i = 0; i < entry.Count; i++)
            {
                SpawnEnemy(entry.EnemyData);
                _remainingEnemies++;
                yield return new WaitForSeconds(entry.SpawnInterval);
            }
        }
        _spawningDone = true;
        CheckWaveComplete();
    }

    private void SpawnEnemy(EnemyData data)
    {
        Vector3 spawnPos = _gridManager.CellToWorld(
            new Vector2Int(Constants.GridEntryColumn, Constants.GridEntryRow));
        EnemyBase enemy = _enemyPool.Get(data);
        enemy.transform.position = spawnPos;
    }

    private void HandleEnemyRemoved(EnemyBase enemy)
    {
        if (!_waveActive) return;
        _remainingEnemies--;
        CheckWaveComplete();
    }

    private void CheckWaveComplete()
    {
        if (_spawningDone && _remainingEnemies <= 0)
        {
            _waveActive = false;
            OnWaveComplete?.Invoke();
        }
    }
}
