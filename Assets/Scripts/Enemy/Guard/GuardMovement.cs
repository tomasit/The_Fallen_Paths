using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static EnemyInfo;

public class GuardMovement : AEnemyMovement
{
    private Transform player;

    void Start() 
    {
        player = ((PlayerController)FindObjectOfType(typeof(PlayerController))).transform;
        detectionManager = GetComponent<EnemyDetectionManager>();
        interactionManager = GetComponent<AEnemyInteraction>();
        agentMovement = GetComponent<Agent>();
        detectionTrigger = GetComponent<TriggerCoroutineProcessor>();
    }

    void Update()
    {
        Move();
        AllowedMovement();
    }
    public override void BasicMovement()
    {
        NoNegative(speed = Speed[EnemyType.Guard]);
    }

    public override void AlertMovement()
    {
        Vector3 targetDirection = FindTargetDirection(player.position);
        target = player;
        
        NoNegative(speed = Speed[EnemyType.Guard] - (Speed[EnemyType.Guard] * 0.5f));
        if (targetDirection.x > 0) {
            if (targetDirection.x < EnemyInfo.DistanceToInteract &&
                RangeOf(targetDirection.y, 0f, 0.80f)) {
                detectionManager.SetState(DetectionState.Spoted);
            }
        }
        if (targetDirection.x < 0) {
            if (targetDirection.x > -EnemyInfo.DistanceToInteract &&
                RangeOf(targetDirection.y, 0f, 0.80f)) {
                detectionManager.SetState(DetectionState.Spoted);
            }
        }
    }

    public override void SpotMovement()
    {
        Vector3 targetDirection = FindTargetDirection(player.position);
        target = player;
        //Debug.Log("targetDirection.y = " + targetDirection.y);
        //Debug.Log("player.position.y = " + player.position.y);
        //Debug.Log("enemy.position.y = " + transform.position.y);
        //Debug.Log("----------------------");

        if (targetDirection.x > 0) {
            if (targetDirection.x < EnemyInfo.DistanceToInteract &&
                RangeOf(targetDirection.y, 0f, 1f)) {
                interactionManager.isAtdistanceToInteract = true;
                speed = 0f;
            } else {
                interactionManager.isAtdistanceToInteract = false;
                NoNegative(speed = Speed[EnemyType.Guard] + (Speed[EnemyType.Guard] * 1.5f));
            }
        }
        if (targetDirection.x < 0) {
            if (targetDirection.x > -EnemyInfo.DistanceToInteract && 
                RangeOf(targetDirection.y, 0f, 1f)) {
                interactionManager.isAtdistanceToInteract = true;
                speed = 0f;
            } else {
                interactionManager.isAtdistanceToInteract = false;
                NoNegative(speed = Speed[EnemyType.Guard] + (Speed[EnemyType.Guard] * 1.5f));
            }
        }
    }
}
