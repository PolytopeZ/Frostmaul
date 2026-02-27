using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TowerMenu : MonoBehaviour
{
    [SerializeField] private UIDocument _document;
    [SerializeField] private TowerPlacer _towerPlacer;
    [SerializeField] private PlayerResources _playerResources;
    [SerializeField] private TowerData[] _availableTowers;

    public static bool IsOpen { get; private set; }

    private VisualElement _panel;
    private ScrollView _scroll;
    private VisualElement _sellPanel;
    private Label _sellInfoLabel;
    private Vector2Int _sellTargetCell;


    private readonly List<(Button btn, TowerData data)> _buttons
        = new List<(Button, TowerData)>();

    private void Awake()
    {
        VisualElement root = _document.rootVisualElement;
        _panel = root.Q<VisualElement>("tower-menu-panel");
        _scroll = root.Q<ScrollView>("tower-scroll");

        _scroll.contentContainer.style.flexDirection = FlexDirection.Row;
        _scroll.contentContainer.style.paddingLeft = 4f;

        _panel.style.display = DisplayStyle.None;

        _sellPanel = root.Q<VisualElement>("sell-panel");
        _sellInfoLabel = root.Q<Label>("sell-info-label");

        _sellPanel.style.display = DisplayStyle.None;

        Button sellBtn = root.Q<Button>("sell-confirm-btn");
        sellBtn.clicked += OnSellConfirmed;

        Button cancelBuildBtn = root.Q<Button>("cancel-build-btn");
        cancelBuildBtn.clicked += _towerPlacer.CancelSelection;

        Button cancelSellBtn = root.Q<Button>("cancel-sell-btn");
        cancelSellBtn.clicked += _towerPlacer.CancelSelection;

        BuildButtons();
    }

    private void OnEnable()
    {
        TowerPlacer.OnCellSelected += HandleCellSelected;
        TowerPlacer.OnSelectionCleared += HandleSelectionCleared;
        PlayerResources.OnGoldChanged += RefreshAffordability;
        TowerPlacer.OnTowerCellTapped += HandleTowerCellTapped;
    }

    private void OnDisable()
    {
        TowerPlacer.OnCellSelected -= HandleCellSelected;
        TowerPlacer.OnSelectionCleared -= HandleSelectionCleared;
        PlayerResources.OnGoldChanged -= RefreshAffordability;
        TowerPlacer.OnTowerCellTapped -= HandleTowerCellTapped;
    }

    // ── Button Construction ───────────────────────────────────────────────────

    private void BuildButtons()
    {
        foreach (TowerData data in _availableTowers)
        {
            TowerData captured = data;

            Button btn = new Button(() => OnTowerChosen(captured));
            btn.style.width = 88f;
            btn.style.height = 120f;
            btn.style.marginRight = 8f;
            btn.style.flexDirection = FlexDirection.Column;
            btn.style.alignItems = Align.Center;
            btn.style.justifyContent = Justify.Center;
            btn.style.backgroundColor = new StyleColor(new Color(0.18f, 0.22f, 0.28f));
            btn.style.borderTopLeftRadius = 8f;
            btn.style.borderTopRightRadius = 8f;
            btn.style.borderBottomLeftRadius = 8f;
            btn.style.borderBottomRightRadius = 8f;

            VisualElement icon = new VisualElement();
            icon.style.width = 52f;
            icon.style.height = 52f;
            icon.style.backgroundColor = new StyleColor(new Color(0.35f, 0.6f, 0.9f));
            icon.style.marginBottom = 6f;
            icon.style.borderTopLeftRadius = 6f;
            icon.style.borderTopRightRadius = 6f;
            icon.style.borderBottomLeftRadius = 6f;
            icon.style.borderBottomRightRadius = 6f;

            Label nameLabel = new Label(data.DisplayName);
            nameLabel.style.color = new StyleColor(Color.white);
            nameLabel.style.fontSize = 11f;
            nameLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            nameLabel.style.whiteSpace = WhiteSpace.Normal;
            nameLabel.style.width = 80f;

            Label costLabel = new Label($"${data.Cost}");
            costLabel.style.color = new StyleColor(new Color(1f, 0.84f, 0.2f));
            costLabel.style.fontSize = 14f;
            costLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            costLabel.style.marginTop = 2f;

            btn.Add(icon);
            btn.Add(nameLabel);
            btn.Add(costLabel);
            _scroll.Add(btn);
            _buttons.Add((btn, captured));
        }
    }

    // ── Callbacks ─────────────────────────────────────────────────────────────

    private void OnTowerChosen(TowerData data)
    {
        _towerPlacer.SetTowerData(data);
        _towerPlacer.PlaceCurrentSelection();
    }

    private void HandleCellSelected(Vector2Int cell)
    {
        IsOpen = true;
        _panel.style.display = DisplayStyle.Flex;
        RefreshAffordability(_playerResources.Gold);
    }

    private void HandleSelectionCleared()
    {
        IsOpen = false;
        _panel.style.display = DisplayStyle.None;
        _sellPanel.style.display = DisplayStyle.None;
    }

    private void HandleTowerCellTapped(Vector2Int cell)
    {
        TowerBase tower = _towerPlacer.GetTowerAt(cell);
        if (tower == null) return;

        _sellTargetCell = cell;
        int refund = tower.Data.Cost / 2;
        _sellInfoLabel.text = $"Sell {tower.Data.DisplayName} for ${refund}?";

        _sellPanel.style.display = DisplayStyle.Flex;
        IsOpen = true;
    }

    private void OnSellConfirmed()
    {
        _towerPlacer.SellTower(_sellTargetCell);
        _sellPanel.style.display = DisplayStyle.None;
        IsOpen = false;
    }

    private void RefreshAffordability(int gold)
    {
        foreach (var (btn, data) in _buttons)
        {
            bool canAfford = gold >= data.Cost;
            btn.SetEnabled(canAfford);
            btn.style.opacity = canAfford ? 1f : 0.38f;
        }
    }
}
