using System;
using UnityEngine;

[Serializable]
public class CardEffect
{
    [SerializeField] private CardEffectType _effectType;
    [SerializeField] private float _value;
    [SerializeField] private TowerData _targetTower;       // UnlockTower only
    [SerializeField] private TowerType _targetTowerType;   // TowerModifier types only

    public CardEffectType EffectType => _effectType;
    public float Value => _value;
    public TowerData TargetTower => _targetTower;
    public TowerType TargetTowerType => _targetTowerType;
}
