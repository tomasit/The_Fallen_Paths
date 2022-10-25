using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static EnemyInfo;

public class CivilianMovement : AEnemyMovement
{
    private Transform player;

    void Start() 
    {
        player = ((PlayerController)FindObjectOfType(typeof(PlayerController))).transform;
        detectionManager = GetComponent<EnemyDetectionManager>();
        interactionManager = GetComponent<AEnemyInteraction>();
        agentMovement = GetComponent<Agent>();
        //enemy = transform.GetChild(0).gameObject;
    }

    void Update()
    {
        Move();
        AllowedMovement();
    }
    public override void BasicMovement()
    {
        NoNegative(speed = Speed[EnemyType.Random]);
    }

    public override void AlertMovement()
    {
        Vector3 targetDirection = FindTargetDirection(player.position);
        target = player;
        
        NoNegative(speed = Speed[EnemyType.Random] - (Speed[EnemyType.Random] * 0.5f));
        if (targetDirection.x > 0) {
            if (targetDirection.x < DistanceToInteract[EnemyType.Random] &&
                ApproximateCoordinates(targetDirection.y, 0f, 0.20f)) {
                detectionManager.SetState(DetectionState.Spoted);
            }
        }
        if (targetDirection.x < 0) {
            if (targetDirection.x > -DistanceToInteract[EnemyType.Random] &&
                ApproximateCoordinates(targetDirection.y, 0f, 0.20f)) {
                detectionManager.SetState(DetectionState.Spoted);
            }
        }
    }

    public override void SpotMovement()
    {
        (Vector3 targetDirection, GameObject enemy) = FindNearestEnemy(typeof(GuardMovement));
        target = enemy.transform;

        if (enemy == null) {
            targetDirection = -1 * FindTargetDirection(player.position);
            target = player;
            //Debug.Log("oposé du player");
        } else {
            //Debug.Log("va alert 'autre enemi");
        }

        if (targetDirection.x > 0) {
            if (targetDirection.x < DistanceToInteract[EnemyType.Random] &&
                ApproximateCoordinates(targetDirection.y, 0f, 0.20f) && 
                enemy != null) {
                interactionManager.isAtdistanceToInteract = true;
                speed = 0f;
            } else {
                interactionManager.isAtdistanceToInteract = false;
                NoNegative(speed = Speed[EnemyType.Random] + (Speed[EnemyType.Random] * 1.5f));
            }
        }
        if (targetDirection.x < 0) {
            if (targetDirection.x > -DistanceToInteract[EnemyType.Random] &&
                ApproximateCoordinates(targetDirection.y, 0f, 0.20f) && 
                enemy != null) {
                interactionManager.isAtdistanceToInteract = true;
                speed = 0f;
            } else {
                interactionManager.isAtdistanceToInteract = false;
                NoNegative(speed = Speed[EnemyType.Random] + (Speed[EnemyType.Random] * 1.5f));
            }
        }
    }
}
