using UnityEngine;

[CreateAssetMenu(fileName = "NewCardData", menuName = "Frostmaul/Card Data")]
public class CardData : ScriptableObject
{
    // ── Identity ──────────────────────────────────────────────────────────────
    [SerializeField] private string _displayName;
    [SerializeField][TextArea(2, 4)] private string _description;
    [SerializeField] private Sprite _icon;

    // ── Classification ────────────────────────────────────────────────────────
    [SerializeField] private CardType _cardType;
    [SerializeField] private CardRarity _rarity;

    // ── Effects ───────────────────────────────────────────────────────────────
    [SerializeField] private CardEffect[] _effects;

    // ── Draw Pool ─────────────────────────────────────────────────────────────
    [SerializeField][Min(1)] private int _weight = 10; // higher = drawn more often

    // ── Properties ────────────────────────────────────────────────────────────
    public string DisplayName => _displayName;
    public string Description => _description;
    public Sprite Icon => _icon;
    public CardType CardType => _cardType;
    public CardRarity Rarity => _rarity;
    public CardEffect[] Effects => _effects;
    public int Weight => _weight;
}
