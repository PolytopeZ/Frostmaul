
using System;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] private EnemyData _data;

    private FlowFieldCalculator _flowField;
    private GridManager _gridManager;
    private float _currentHp;
    private bool _reachedExit;

    public EnemyData Data => _data;

    public static event Action<EnemyBase> OnEnemyKilled;
    public static event Action<EnemyBase> OnEnemyReachedExit;

    // ── Unity Lifecycle ───────────────────────────────────────────────────────

    private void Start()
    {
        // TODO EnemyPool: inject these references from the pool instead
        _flowField = FindObjectOfType<FlowFieldCalculator>();
        _gridManager = FindObjectOfType<GridManager>();
        _currentHp = _data.MaxHp;
    }

    private void Update()
    {
        if (_reachedExit) return;
        Move();
        CheckExit();
    }

    // ── Navigation ────────────────────────────────────────────────────────────

    private void Move()
    {
        Vector2 direction;

        if (_data.IsFlyer)
        {
            Vector3 exitWorld = _gridManager.CellToWorld(
                new Vector2Int(Constants.GridExitColumn, Constants.GridExitRow));
            direction = ((Vector2)(exitWorld - transform.position)).normalized;
        }
        else
        {
            Vector2Int cell = _gridManager.WorldToCell(transform.position);
            direction = _flowField.GetDirection(cell);
            if (direction == Vector2.zero) return;
        }

        transform.position += (Vector3)(direction * _data.Speed * Time.deltaTime);
    }

    private void CheckExit()
    {
        Vector3 exitWorld = _gridManager.CellToWorld(
            new Vector2Int(Constants.GridExitColumn, Constants.GridExitRow));

        if (Vector2.Distance(transform.position, exitWorld) < Constants.EnemyExitThreshold)
        {
            _reachedExit = true;
            OnEnemyReachedExit?.Invoke(this);
            Destroy(gameObject); // TODO EnemyPool: return to pool instead
        }
    }

    // ── Combat ────────────────────────────────────────────────────────────────

    public void TakeDamage(float damage)
    {
        float effective = Mathf.Max(0f, damage - _data.Armor);
        _currentHp -= effective;
        if (_currentHp <= 0f) Die();
    }

    private void Die()
    {
        OnEnemyKilled?.Invoke(this);
        Destroy(gameObject); // TODO EnemyPool: return to pool instead
    }
}
