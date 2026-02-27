using UnityEngine;
using UnityEngine.UIElements;

public class CardDrawUI : MonoBehaviour
{
    [SerializeField] private UIDocument _document;
    [SerializeField] private CardEffectApplier _applier;
    [SerializeField] private CardPoolManager _cardPool;

    private VisualElement _root;
    private VisualElement _cardRow;
    private Button _rerollBtn;
    private CardData[] _currentOffer;

    private void Awake()
    {
        VisualElement root = _document.rootVisualElement;
        _root = root.Q<VisualElement>("card-draw-root");
        _cardRow = root.Q<VisualElement>("card-row");
        _rerollBtn = root.Q<Button>("reroll-btn");

        _root.style.display = DisplayStyle.None;
        _rerollBtn.clicked += OnReroll;
    }

    private void OnEnable()
    {
        CardPoolManager.OnDrawReady += ShowCards;
        WaveManager.OnPhaseChanged += HandlePhaseChanged;
    }

    private void OnDisable()
    {
        CardPoolManager.OnDrawReady -= ShowCards;
        WaveManager.OnPhaseChanged -= HandlePhaseChanged;
    }

    // ── Event Handlers ────────────────────────────────────────────────────────

    private void HandlePhaseChanged(GamePhase phase)
    {
        if (phase != GamePhase.Reward)
            Hide();
    }

    private void ShowCards(CardData[] cards)
    {
        _currentOffer = cards;
        BuildCardButtons(cards);
        RefreshRerollButton();
        _root.style.display = DisplayStyle.Flex;
    }

    private void OnReroll()
    {
        if (RunManager.Current.RerollsRemaining <= 0) return;
        CardData[] newOffer = _cardPool.Reroll(_currentOffer);
        if (newOffer.Length == 0) return; // pool exhausted — keep current offer, don't spend re-roll
        RunManager.Current.RerollsRemaining--;
        _currentOffer = newOffer;
        BuildCardButtons(_currentOffer);
        RefreshRerollButton();
    }

    private void OnCardPicked(CardData card)
    {
        _applier.Apply(card);
        Hide();
    }

    private void Hide()
    {
        _root.style.display = DisplayStyle.None;
        _cardRow.Clear();
    }

    // ── Card Button Construction ──────────────────────────────────────────────

    private void BuildCardButtons(CardData[] cards)
    {
        _cardRow.Clear();
        foreach (CardData card in cards)
        {
            CardData captured = card;
            Button btn = new Button(() => OnCardPicked(captured));
            btn.style.width = 140f;
            btn.style.minHeight = 200f;
            btn.style.marginLeft = 8f;
            btn.style.marginRight = 8f;
            btn.style.flexDirection = FlexDirection.Column;
            btn.style.alignItems = Align.Center;
            btn.style.justifyContent = Justify.FlexStart;
            btn.style.paddingTop = 12f;
            btn.style.paddingBottom = 12f;
            btn.style.backgroundColor = new StyleColor(new Color(0.12f, 0.16f, 0.22f));
            btn.style.borderTopLeftRadius = 10f;
            btn.style.borderTopRightRadius = 10f;
            btn.style.borderBottomLeftRadius = 10f;
            btn.style.borderBottomRightRadius = 10f;

            Color rarityColor = RarityColor(card.Rarity);

            VisualElement strip = new VisualElement();
            strip.style.width = 120f;
            strip.style.height = 4f;
            strip.style.backgroundColor = new StyleColor(rarityColor);
            strip.style.borderTopLeftRadius = 2f;
            strip.style.borderTopRightRadius = 2f;
            strip.style.marginBottom = 10f;

            Label nameLabel = new Label(card.DisplayName);
            nameLabel.style.color = new StyleColor(Color.white);
            nameLabel.style.fontSize = 14f;
            nameLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            nameLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            nameLabel.style.whiteSpace = WhiteSpace.Normal;
            nameLabel.style.width = 120f;
            nameLabel.style.marginBottom = 6f;

            Label rarityLabel = new Label(card.Rarity.ToString());
            rarityLabel.style.color = new StyleColor(rarityColor);
            rarityLabel.style.fontSize = 11f;
            rarityLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            rarityLabel.style.marginBottom = 10f;

            Label descLabel = new Label(card.Description);
            descLabel.style.color = new StyleColor(new Color(0.8f, 0.8f, 0.8f));
            descLabel.style.fontSize = 11f;
            descLabel.style.unityTextAlign = TextAnchor.UpperCenter;
            descLabel.style.whiteSpace = WhiteSpace.Normal;
            descLabel.style.width = 120f;

            btn.Add(strip);
            btn.Add(nameLabel);
            btn.Add(rarityLabel);
            btn.Add(descLabel);
            _cardRow.Add(btn);
        }
    }

    private void RefreshRerollButton()
    {
        int remaining = RunManager.Current?.RerollsRemaining ?? 0;
        bool canReroll = remaining > 0;
        _rerollBtn.SetEnabled(canReroll);
        _rerollBtn.text = canReroll ? $"Re-roll ({remaining} remaining)" : "Re-roll (none left)";
        _rerollBtn.style.opacity = canReroll ? 1f : 0.4f;
    }

    private static Color RarityColor(CardRarity rarity) => rarity switch
    {
        CardRarity.Common => new Color(0.7f, 0.7f, 0.7f),
        CardRarity.Uncommon => new Color(0.3f, 0.8f, 0.3f),
        CardRarity.Rare => new Color(0.3f, 0.5f, 1.0f),
        CardRarity.Legendary => new Color(1.0f, 0.7f, 0.1f),
        _ => Color.white
    };
}
