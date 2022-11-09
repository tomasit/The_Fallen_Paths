using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyInfo
{
    public static Dictionary<EnemyType, float> Speed = new Dictionary<EnemyType, float>()
    {
        { EnemyType.Guard, 1.5f },
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

    public static float DistanceToInteract = 1f;

    public static bool RangeOfPosition(Vector3 posAim, Vector3 posCompare, float range) 
    {
        //Debug.Log(posCompare.x - range + " < " + posAim.x + " && " + posAim.x + " < " + posCompare.x + range);
        //Debug.Log(posCompare.y - range + " < " + posAim.y + " && " + posAim.y + " < " + posCompare.y + range);
        return (
            (posCompare.x - range <= posAim.x) && (posAim.x <= posCompare.x + range) && 
            (posCompare.y - range <= posAim.y) && (posAim.y <= posCompare.y + range)
        );
    }

    public static bool RangeOf(float posAim, float posCompare, float range)
    {
        //Debug.Log(posCompare - range + " < " + posAim + " && " + posAim + " < " + posCompare + range);
        return (posCompare - range <= posAim) && (posAim <= posCompare + range);
    }

    public static Vector3 FindTargetDirection(Vector3 position, Vector3 targetPosition)
    {
        return targetPosition - position;
    }
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
    Flee,
    Freeze,
}

public enum EnemyEventState {
    None,
    SeenPlayer,
    FightPlayer,
    NoGuardAround,
    SeenRandomSpoted,
    SeenDeadBody,
    SeenOffLight,
}

[Serializable]
public struct Enemy {
    public string uuid;
    public bool enabled;
    public EnemyType type;

    public SpriteRenderer sprite;
    public Animator animator;

    public RoomProprieties roomProprieties;
    public RoomProprieties fleePoints;

    public Agent agentMovement;
    public AEnemyMovement movementManager;
    public EnemyDetectionManager detectionManager;
}
