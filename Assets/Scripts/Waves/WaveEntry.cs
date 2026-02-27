using UnityEngine;

[System.Serializable]
public class WaveEntry
{
    [SerializeField] private EnemyData _enemyData;
    [SerializeField][Min(1)] private int _count;
    [SerializeField][Min(0.1f)] private float _spawnInterval;

    public EnemyData EnemyData => _enemyData;
    public int Count => _count;
    public float SpawnInterval => _spawnInterval;
}
