using System.Collections.Generic;
using UnityEngine;

public static class G
{
    public static Main main;
    public static UI ui;
    public static ScreenShake screenShake;
    public static Transform circleCenter;
    public static GameObject etherOrbPrefab;
    public static GameObject damagePopupPrefab;

    public static bool IsPaused;

    public static int Ether;
    //

    private static readonly HashSet<string> unlockedEnemyKeys = new HashSet<string>();

    public static int ClickDamage = 1;
    public static float CritChance = 0.1f;
    public static float CritMultiplier = 2f;
    public static float ClickRadius = 0.5f;

    public static event System.Action<int> OnEtherChanged;
    public static event System.Action<string> OnEnemyUnlocked;

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

    public static bool IsEnemyUnlocked(string unlockKey, bool unlockedByDefault)
    {
        if (unlockedByDefault) return true;
        if (string.IsNullOrEmpty(unlockKey)) return true;
        
        return unlockedEnemyKeys.Contains(unlockKey);
    }

    public static void UnlockEnemy(string unlockKey)
    {
        if(string.IsNullOrEmpty(unlockKey)) return;

        if (unlockedEnemyKeys.Add(unlockKey))
            OnEnemyUnlocked?.Invoke(unlockKey);
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