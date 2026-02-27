using System;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private WaveData[] _waves;
    [SerializeField] private WaveSpawner _waveSpawner;
    [SerializeField] private PlayerResources _playerResources;
    [SerializeField] private float _buildPhaseDuration = 15f;
    [SerializeField] private float _rewardPhaseDuration = 3f;

    private GamePhase _phase;
    private int _currentWaveIndex;
    private float _phaseTimer;
    private bool _cardDrawPending;

    public GamePhase Phase => _phase;
    public int CurrentWaveIndex => _currentWaveIndex;
    public float PhaseTimer => _phaseTimer;

    public static event Action<GamePhase> OnPhaseChanged;
    public static event Action<int> OnWaveStarted;  // wave index (0-based)
    public static event Action<bool> OnGameOver;     // true = victory

    private void OnEnable()
    {
        WaveSpawner.OnWaveComplete += HandleWaveComplete;
        PlayerResources.OnLivesChanged += HandleLivesChanged;
        CardPoolManager.OnDrawReady += HandleDrawReady;
        CardEffectApplier.OnCardApplied += HandleCardApplied;
    }

    private void OnDisable()
    {
        WaveSpawner.OnWaveComplete -= HandleWaveComplete;
        PlayerResources.OnLivesChanged -= HandleLivesChanged;
        CardPoolManager.OnDrawReady -= HandleDrawReady;
        CardEffectApplier.OnCardApplied -= HandleCardApplied;
    }

    private void Start()
    {
        _currentWaveIndex = 0;
        EnterBuildPhase();
    }

    private void Update()
    {
        if (_phase == GamePhase.Build)
        {
            _phaseTimer -= Time.deltaTime;
            if (_phaseTimer <= 0f)
                StartWave();
        }
        else if (_phase == GamePhase.Reward)
        {
            if (!_cardDrawPending)
            {
                _phaseTimer -= Time.deltaTime;
                if (_phaseTimer <= 0f)
                    AdvanceWave();
            }
        }
    }

    // ── Phase Transitions ─────────────────────────────────────────────────────

    private void EnterBuildPhase()
    {
        _phase = GamePhase.Build;
        _phaseTimer = _buildPhaseDuration;
        OnPhaseChanged?.Invoke(_phase);
    }

    // Called by timer or future HUD "Start Wave" button
    public void StartWave()
    {
        if (_phase != GamePhase.Build) return;
        if (_currentWaveIndex >= _waves.Length) return;

        _phase = GamePhase.Wave;
        OnPhaseChanged?.Invoke(_phase);
        OnWaveStarted?.Invoke(_currentWaveIndex);
        _waveSpawner.StartWave(_waves[_currentWaveIndex]);
    }

    private void HandleWaveComplete()
    {
        if (_phase != GamePhase.Wave) return;

        _playerResources.AddGold(_waves[_currentWaveIndex].GoldReward);

        _phase = GamePhase.Reward;
        _phaseTimer = _rewardPhaseDuration;
        OnPhaseChanged?.Invoke(_phase);
    }

    private void AdvanceWave()
    {
        _currentWaveIndex++;
        if (_currentWaveIndex >= _waves.Length)
            TriggerGameOver(true);
        else
            EnterBuildPhase();
    }

    private void HandleLivesChanged(int lives)
    {
        if (lives <= 0)
            TriggerGameOver(false);
    }

    private void HandleDrawReady(CardData[] _) => _cardDrawPending = true;

    private void HandleCardApplied(CardData _)
    {
        _cardDrawPending = false;
        if (_phase == GamePhase.Reward)
            AdvanceWave();
    }

    private void TriggerGameOver(bool victory)
    {
        if (_phase == GamePhase.GameOver) return;
        _phase = GamePhase.GameOver;
        _waveSpawner.StopWave();
        OnPhaseChanged?.Invoke(_phase);
        OnGameOver?.Invoke(victory);
    }
}
