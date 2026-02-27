using System;
using UnityEngine;

public class CardEffectApplier : MonoBehaviour
{
    [SerializeField] private CardPoolManager _cardPool;
    [SerializeField] private PlayerResources _playerResources;

    public static event Action<CardData> OnCardApplied;

    public void Apply(CardData card)
    {
        RunState run = RunManager.Current;
        foreach (CardEffect effect in card.Effects)
            ApplyEffect(effect, run, card);

        _cardPool.MarkPicked(card);
        OnCardApplied?.Invoke(card);
    }

    private void ApplyEffect(CardEffect effect, RunState run, CardData card)
    {
        switch (effect.EffectType)
        {
            case CardEffectType.UnlockTower:
                if (effect.TargetTower != null && !run.UnlockedTowers.Contains(effect.TargetTower))
                    run.UnlockedTowers.Add(effect.TargetTower);
                break;

            case CardEffectType.TowerDamagePercent:
                run.GetTowerMods(effect.TargetTowerType).DamageMultiplier *= 1f + effect.Value;
                break;

            case CardEffectType.TowerRangePercent:
                run.GetTowerMods(effect.TargetTowerType).RangeMultiplier *= 1f + effect.Value;
                break;

            case CardEffectType.TowerAttackSpeedPercent:
                run.GetTowerMods(effect.TargetTowerType).AttackSpeedMultiplier *= 1f + effect.Value;
                break;

            case CardEffectType.TowerCostFlat:
                run.GetTowerMods(effect.TargetTowerType).CostReduction += (int)effect.Value;
                break;

            case CardEffectType.GlobalDamagePercent:
                run.GlobalDamageMultiplier *= 1f + effect.Value;
                break;

            case CardEffectType.GlobalGoldPerKill:
                run.ExtraGoldPerKill += (int)effect.Value;
                break;

            case CardEffectType.BonusLives:
                _playerResources.AddLives((int)effect.Value);
                break;

            case CardEffectType.AddGridRow:
                run.BonusGridRows += (int)effect.Value;
                break;

            case CardEffectType.RelicPassive:
                if (!run.ActiveRelics.Contains(card))
                    run.ActiveRelics.Add(card);
                break;

            case CardEffectType.EnemySpeedPercent:
                run.EnemySpeedMultiplier *= 1f + effect.Value;
                break;

            case CardEffectType.EnemyHpPercent:
                run.EnemyHpMultiplier *= 1f + effect.Value;
                break;

            case CardEffectType.SkipNextBoss:
                run.SkipNextBoss = true;
                break;
        }
    }
}
