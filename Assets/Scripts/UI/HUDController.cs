using UnityEngine;
using UnityEngine.UIElements;

public class HUDController : MonoBehaviour
{
    [SerializeField] private UIDocument _document;
    [SerializeField] private PlayerResources _playerResources;
    [SerializeField] private WaveManager _waveManager;

    private Label _livesLabel;
    private Label _waveLabel;
    private Label _phaseLabel;
    private Label _goldLabel;

    private void Awake()
    {
        VisualElement root = _document.rootVisualElement;
        _livesLabel = root.Q<Label>("lives-label");
        _waveLabel = root.Q<Label>("wave-label");
        _phaseLabel = root.Q<Label>("phase-label");
        _goldLabel = root.Q<Label>("gold-label");
    }

    private void Start()
    {
        UpdateLives(_playerResources.Lives);
        UpdateGold(_playerResources.Gold);
        UpdateWave(_waveManager.CurrentWaveIndex);
        UpdatePhase(_waveManager.Phase);
    }

    private void OnEnable()
    {
        PlayerResources.OnLivesChanged += UpdateLives;
        PlayerResources.OnGoldChanged += UpdateGold;
        WaveManager.OnWaveStarted += UpdateWave;
        WaveManager.OnPhaseChanged += UpdatePhase;
    }

    private void OnDisable()
    {
        PlayerResources.OnLivesChanged -= UpdateLives;
        PlayerResources.OnGoldChanged -= UpdateGold;
        WaveManager.OnWaveStarted -= UpdateWave;
        WaveManager.OnPhaseChanged -= UpdatePhase;
    }

    private void UpdateLives(int lives) => _livesLabel.text = $"Lives: {lives}";
    private void UpdateGold(int gold) => _goldLabel.text = $"Gold: {gold}";
    private void UpdateWave(int index) => _waveLabel.text = $"Wave {index + 1}";

    private void UpdatePhase(GamePhase phase)
    {
        _phaseLabel.text = phase switch
        {
            GamePhase.Build => "Build",
            GamePhase.Wave => "Wave",
            GamePhase.Reward => "Reward!",
            _ => ""
        };
    }
}
