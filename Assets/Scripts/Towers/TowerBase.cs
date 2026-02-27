using UnityEngine;

public class TowerBase : MonoBehaviour
{
    [SerializeField] private TowerData _data;

    private Transform _currentTarget;
    private float _attackCooldown;
    private float _targetingTimer;
    private int _enemyLayerMask;

    private static readonly Collider2D[] s_HitBuffer = new Collider2D[32];

    public TowerData Data => _data;

    // ── Unity Lifecycle ───────────────────────────────────────────────────────

    private void Start()
    {
        _enemyLayerMask = LayerMask.GetMask(Constants.EnemyLayerName);
        _targetingTimer = 0f;
        _attackCooldown = 0f;
    }

    private void Update()
    {
        _targetingTimer -= Time.deltaTime;
        if (_targetingTimer <= 0f)
        {
            _targetingTimer = Constants.TowerTargetingInterval;
            FindTarget();
        }

        if (_currentTarget == null) return;

        _attackCooldown -= Time.deltaTime;
        if (_attackCooldown <= 0f)
        {
            Attack();
            _attackCooldown = 1f / _data.AttackSpeed;
        }
    }

    // ── Targeting ─────────────────────────────────────────────────────────────

    private void FindTarget()
    {
        int count = Physics2D.OverlapCircleNonAlloc(
            transform.position, _data.Range, s_HitBuffer, _enemyLayerMask);

        if (count == 0)
        {
            _currentTarget = null;
            return;
        }

        float nearestSqrDist = float.MaxValue;
        Transform nearest = null;

        for (int i = 0; i < count; i++)
        {
            Vector2 offset = s_HitBuffer[i].transform.position - transform.position;
            float sqrDist = offset.sqrMagnitude;
            if (sqrDist < nearestSqrDist)
            {
                nearestSqrDist = sqrDist;
                nearest = s_HitBuffer[i].transform;
            }
        }

        _currentTarget = nearest;
    }

    // ── Attack ────────────────────────────────────────────────────────────────

    private void Attack()
    {
        // TODO Projectile T1: spawn projectile toward _currentTarget
        // Damage is applied by the projectile on contact, not here
    }

    // ── Editor Gizmos ────────────────────────────────────────────────────────
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (_data == null) return;
        Gizmos.color = new Color(1f, 1f, 0f, 0.25f);
        Gizmos.DrawWireSphere(transform.position, _data.Range);
    }
#endif
}
