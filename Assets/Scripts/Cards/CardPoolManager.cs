using System.Collections.Generic;
using UnityEngine;

public class CardPoolManager : MonoBehaviour
{
    [SerializeField] private CardData[] _allCards;

    public static event System.Action<CardData[]> OnDrawReady;

    private readonly HashSet<CardData> _drawnThisRun = new HashSet<CardData>();

    private void Start()
    {
        _drawnThisRun.Clear();
    }

    private void OnEnable()
    {
        WaveManager.OnPhaseChanged += HandlePhaseChanged;
    }

    private void OnDisable()
    {
        WaveManager.OnPhaseChanged -= HandlePhaseChanged;
    }

    // ── Phase Listener ────────────────────────────────────────────────────────

    private void HandlePhaseChanged(GamePhase phase)
    {
        if (phase != GamePhase.Reward) return;
        CardData[] drawn = Draw();
        if (drawn.Length > 0)
            OnDrawReady?.Invoke(drawn);
    }

    // ── Public API ────────────────────────────────────────────────────────────

    public CardData[] Draw()
    {
        return WeightedSample(_drawnThisRun, null, Constants.CardDrawCount);
    }

    public CardData[] Reroll(CardData[] previousOffer)
    {
        return WeightedSample(_drawnThisRun, new HashSet<CardData>(previousOffer), Constants.CardDrawCount);
    }

    public void MarkPicked(CardData card)
    {
        _drawnThisRun.Add(card);
    }

    // ── Weighted Sampling ─────────────────────────────────────────────────────

    private CardData[] WeightedSample(HashSet<CardData> exclude, HashSet<CardData> alsoExclude, int count)
    {
        var eligible = new List<CardData>(_allCards.Length);
        foreach (CardData card in _allCards)
        {
            if (exclude.Contains(card)) continue;
            if (alsoExclude != null && alsoExclude.Contains(card)) continue;
            eligible.Add(card);
        }

        int take = Mathf.Min(count, eligible.Count);
        var result = new CardData[take];
        var pickedThisDraw = new HashSet<CardData>();

        for (int i = 0; i < take; i++)
        {
            int totalWeight = 0;
            foreach (CardData c in eligible)
                if (!pickedThisDraw.Contains(c))
                    totalWeight += c.Weight;

            int roll = Random.Range(0, totalWeight);
            int cumulative = 0;
            foreach (CardData c in eligible)
            {
                if (pickedThisDraw.Contains(c)) continue;
                cumulative += c.Weight;
                if (roll < cumulative)
                {
                    result[i] = c;
                    pickedThisDraw.Add(c);
                    break;
                }
            }
        }

        return result;
    }
}
