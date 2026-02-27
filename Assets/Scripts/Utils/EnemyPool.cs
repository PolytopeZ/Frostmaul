using UnityEngine;
using UnityEngine.Pool;

public class EnemyPool : MonoBehaviour
{
    [SerializeField] private EnemyData[] _registeredTypes;

    private System.Collections.Generic.Dictionary<EnemyData, ObjectPool<EnemyBase>> _pools;

    private void Awake()
    {
        _pools = new System.Collections.Generic.Dictionary<EnemyData, ObjectPool<EnemyBase>>();

        foreach (EnemyData data in _registeredTypes)
        {
            EnemyData capturedData = data;
            _pools[capturedData] = new ObjectPool<EnemyBase>(
                createFunc: () =>
                {
                    EnemyBase e = Instantiate(capturedData.Prefab).GetComponent<EnemyBase>();
                    e.SetPool(_pools[capturedData]);
                    return e;
                },
                actionOnGet: e => { e.gameObject.SetActive(true); e.OnSpawn(); },
                actionOnRelease: e => e.gameObject.SetActive(false),
                actionOnDestroy: e => Destroy(e.gameObject),
                defaultCapacity: 8,
                maxSize: 32
            );
        }
    }

    public EnemyBase Get(EnemyData data)
    {
        if (_pools.TryGetValue(data, out ObjectPool<EnemyBase> pool))
            return pool.Get();

        Debug.LogWarning("EnemyPool: no pool registered for " + data.name);
        return null;
    }
}
