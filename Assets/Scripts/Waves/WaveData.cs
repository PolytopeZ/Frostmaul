using UnityEngine;

[CreateAssetMenu(fileName = "NewWaveData", menuName = "Frostmaul/Wave Data")]
public class WaveData : ScriptableObject
{
    [SerializeField] private string _displayName;
    [SerializeField] private WaveEntry[] _entries;
    [SerializeField][Min(0)] private int _goldReward;

    public string DisplayName => _displayName;
    public WaveEntry[] Entries => _entries;
    public int GoldReward => _goldReward;
}
