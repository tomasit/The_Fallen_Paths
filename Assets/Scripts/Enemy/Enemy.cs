using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyInfo
{
    public static Dictionary<EnemyType, float> EnemySpeed = new Dictionary<EnemyType, float>()
    {
        { EnemyType.Guard, 4f },
        { EnemyType.RoyalGuard, 7f },
        { EnemyType.Random, 1f }
    };
}

public enum EnemyType
{
    Guard,
    RoyalGuard,
    Random,
}

public enum DetectionState
{
    None,
    Alert,
    Spoted,
}

[Serializable]
public struct Enemy {
    public string uuid;
    public EnemyType type;

    public SpriteRenderer sprite;
    public RoomProprieties roomProprieties;
    
    public AEnemyMovement movementManager;
    public EnenmyDectionManager dectionManager;   
    public EnemyAttackManager attackManager;
}
