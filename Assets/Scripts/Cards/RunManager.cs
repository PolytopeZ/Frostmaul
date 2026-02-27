using UnityEngine;

public class RunManager : MonoBehaviour
{
    public static RunState Current { get; private set; }

    private void Awake()
    {
        Current = new RunState();
    }
}
