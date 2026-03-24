using System;
using System.Collections.Generic;
using UnityEngine;

public static class G
{
    public static Main main;
    public static UI ui;
    public static UpgradeTreeManager upgradeTreeManager;
    public static ScreenShake screenShake;
    public static Transform circleCenter;
    public static GameObject etherOrbPrefab;
    public static GameObject damagePopupPrefab;
    public static RitualProgressionManager ritualProgression;
    public static EnemySpawner spawner;
    public static EnemySpawnSet spawnSet;
    public static StoryPanelController storyPanel;
    public static bool IsPaused;

    public static int RedEther;
    public static int WhiteEther;
    public static int PurpleEther;

    private static readonly HashSet<string> unlockedEnemyKeys = new HashSet<string>();

    public static int ClickDamage = 3;
    public static float CritChance = 0.05f;
    public static float CritMultiplier = 2f;
    public static float ClickRadius = 0.3f;

    public static event System.Action<EtherType, int> OnEtherChanged;
    public static event System.Action<string> OnEnemyUnlocked;

    //-------------------------------------------
    public static int GlobalEnemyLimitBonus = 0;
    public static float GlobalSpawnIntervalMultiplier = 1f;
    public static float GlobalEnemyRewardMultiplier = 1f;
    //-------------------------------------------

    public static void AddEther(EtherType type, int v)
    {
        switch (type)
        {
            case EtherType.Red:
                RedEther += v;
                OnEtherChanged?.Invoke(type, RedEther);
                break;
            case EtherType.White:
                WhiteEther += v;
                OnEtherChanged?.Invoke(type, WhiteEther);
                break;
            case EtherType.Purple:
                PurpleEther += v;
                OnEtherChanged?.Invoke(type, PurpleEther);
                break;
        }
    }

    public static bool SpendEther(EtherType type, int v)
    {
        switch (type)
        {
            case EtherType.Red:
                if (RedEther < v) return false;
                RedEther -= v;
                OnEtherChanged?.Invoke(type, RedEther);
                return true;
            case EtherType.White:
                if (WhiteEther < v) return false;
                WhiteEther -= v;
                OnEtherChanged?.Invoke(type, WhiteEther);
                return true;
            case EtherType.Purple:
                if (PurpleEther < v) return false;
                PurpleEther -= v;
                OnEtherChanged?.Invoke(type, PurpleEther);
                return true;
        }

        return false;
    }

    public static bool IsEnemyUnlocked(string unlockKey, bool unlockedByDefault)
    {
        if (unlockedByDefault) return true;
        if (string.IsNullOrEmpty(unlockKey)) return true;

        return unlockedEnemyKeys.Contains(unlockKey);
    }

    public static void UnlockEnemy(string unlockKey)
    {
        if (string.IsNullOrEmpty(unlockKey)) return;

        if (unlockedEnemyKeys.Add(unlockKey))
            OnEnemyUnlocked?.Invoke(unlockKey);
    }

    public static int GetEther(EtherType etherType)
    {
        return etherType switch
        {
            EtherType.Red => RedEther,
            EtherType.White => WhiteEther,
            EtherType.Purple => PurpleEther,
            _ => 0,
        };
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

public enum EtherType
{
    Red,
    White,
    Purple
}