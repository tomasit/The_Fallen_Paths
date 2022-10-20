using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyInfo
{
    public static Dictionary<EnemyType, float> Speed = new Dictionary<EnemyType, float>()
    {
        { EnemyType.Guard, 4f },
        { EnemyType.RoyalGuard, 7f },
        { EnemyType.Random, 1f }
    };

    public static Dictionary<EnemyType, float> DistanceToInteract = new Dictionary<EnemyType, float>()
    {
        { EnemyType.Guard, 2f },
        { EnemyType.RoyalGuard, 3f },
        { EnemyType.Random, 1f }
    };

    public static Dictionary<EnemyType, float> CoolDown = new Dictionary<EnemyType, float>()
    {
        { EnemyType.Guard, 1.5f },
        { EnemyType.RoyalGuard, 2f },
        { EnemyType.Random, 2f }
    };

    public static Dictionary<EnemyType, float> Damage = new Dictionary<EnemyType, float>()
    {
        { EnemyType.Guard, 1f },
        { EnemyType.RoyalGuard, 2f },
        { EnemyType.Random, 0.5f }
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

public enum ActionType 
{
    Attack,
    Alert,
    Climb
}

[Serializable]
public struct Enemy {
    public string uuid;
    public EnemyType type;
    //public Vector3 pos;

    public SpriteRenderer sprite;
    public RoomProprieties roomProprieties;
    
    public AEnemyMovement movementManager;
    public EnemyDetectionManager dectionManager;
    public AEnemyInteraction interactionManager;
}
