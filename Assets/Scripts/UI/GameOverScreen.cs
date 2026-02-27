using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameOverScreen : MonoBehaviour
{
    public static bool IsGameOver { get; private set; }

    private VisualElement _root;
    private Label _resultLabel;

    private void Awake()
    {
        IsGameOver = false;
        VisualElement docRoot = GetComponent<UIDocument>().rootVisualElement;
        _root = docRoot.Q<VisualElement>("gameover-root");
        _resultLabel = docRoot.Q<Label>("result-label");

        Button restartBtn = docRoot.Q<Button>("restart-btn");
        restartBtn.clicked += () =>
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnEnable() => WaveManager.OnGameOver += HandleGameOver;
    private void OnDisable() => WaveManager.OnGameOver -= HandleGameOver;

    private void HandleGameOver(bool victory)
    {
        IsGameOver = true;
        _resultLabel.text = victory ? "Victory!" : "Defeat";
        _resultLabel.style.color = victory
            ? new StyleColor(new Color(0.4f, 1f, 0.4f))
            : new StyleColor(new Color(1f, 0.35f, 0.35f));
        _root.style.display = DisplayStyle.Flex;
    }
}
