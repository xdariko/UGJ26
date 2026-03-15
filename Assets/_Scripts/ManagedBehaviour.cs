using UnityEngine;

public static class G
{
    public static Main main;
    public static UI ui;
    public static Transform circleCenter;
    public static GameObject etherOrbPrefab;

    public static bool IsPaused;

    public static int Ether;
    public static int ClickDamage = 1;
    public static float ClickRadius = 0.5f;

    public static event System.Action<int> OnEtherChanged;

    public static void AddEther(int v)
    {
        Ether += v;
        OnEtherChanged.Invoke(Ether);
    }

    public static bool SpendEther(int v) 
    {
        if (Ether < v) return false;
        Ether -= v;
        OnEtherChanged.Invoke(Ether);
        return true;
    }
}

public class ManagedBehaviour : MonoBehaviour
{
    void Update()
    {
        if (!G.IsPaused)
            PausableUpdate();
    }

    void FixedUpdate()
    {
        if (!G.IsPaused)
            PausableFixedUpdate();
    }

    protected virtual void PausableUpdate() { }
    protected virtual void PausableFixedUpdate() { }
}