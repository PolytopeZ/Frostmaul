using UnityEngine;
using UnityEngine.Pool;

public class ProjectileBase : MonoBehaviour
{
    private IObjectPool<ProjectileBase> _pool;
    private EnemyBase _target;
    private float _damage;
    private float _speed;

    // ── Setup ─────────────────────────────────────────────────────────────────

    public void SetPool(IObjectPool<ProjectileBase> pool)
    {
        _pool = pool;
    }

    public void Initialize(EnemyBase target, float damage, float speed)
    {
        _target = target;
        _damage = damage;
        _speed = speed;
    }

    // ── Movement ──────────────────────────────────────────────────────────────

    private void Update()
    {
        if (_target == null)
        {
            Release();
            return;
        }

        Vector3 toTarget = _target.transform.position - transform.position;
        float dist = toTarget.magnitude;

        if (dist < Constants.ProjectileImpactThreshold)
        {
            _target.TakeDamage(_damage);
            Release();
            return;
        }

        Vector3 dir = toTarget / dist;
        transform.position += dir * _speed * Time.deltaTime;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    // ── Pool ──────────────────────────────────────────────────────────────────

    private void Release()
    {
        _target = null;
        if (_pool != null)
            _pool.Release(this);
        else
            Destroy(gameObject);
    }
}
