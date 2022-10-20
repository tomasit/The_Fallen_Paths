using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static EnemyInfo;

public class CivilianMovement : AEnemyMovement
{
    private Transform player;

    void Start() 
    {
        player = ((PlayerMovementTEST)FindObjectOfType(typeof(PlayerMovementTEST))).transform;
        detectionManager = GetComponent<EnemyDetectionManager>();
        interactionManager = GetComponent<AEnemyInteraction>();
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
        
        NoNegative(speed = Speed[EnemyType.Random] - 0.5f);
        if (targetDirection.x > 0) {
            if (targetDirection.x < DistanceToInteract[EnemyType.Random]) {
                detectionManager.SetState(DetectionState.Spoted);
            }
            direction = 1;
        }
        if (targetDirection.x < 0) {
            if (targetDirection.x > -DistanceToInteract[EnemyType.Random]) {
                detectionManager.SetState(DetectionState.Spoted);
            }
            direction = -1;
        }
    }

    public override void SpotMovement()
    {
        (Vector3 targetDirection, GameObject enemy) = FindNearestEnemy(typeof(GuardMovement));

        if (enemy == null) {
            targetDirection = -1 * FindTargetDirection(player.position);
            //Debug.Log("oposé du player");
        } else {
            //Debug.Log("va alert 'autre enemi");
        }

        if (targetDirection.x > 0) {
            direction = 1;
            // et que le Y est le meme
            if (targetDirection.x < DistanceToInteract[EnemyType.Random] && enemy != null) {
                interactionManager.isAtdistanceToInteract = true;
                speed = 0f;
                //après je sais pas ce que le random peux faire...
            } else {
                interactionManager.isAtdistanceToInteract = false;
                NoNegative(speed = Speed[EnemyType.Random] + 4f);
            }
        }
        if (targetDirection.x < 0) {
            direction = -1;
            // et que le Y est le meme
            if (targetDirection.x > -DistanceToInteract[EnemyType.Random] && enemy != null) {
                interactionManager.isAtdistanceToInteract = true;
                speed = 0f;
                //après je sais pas ce que le random peux faire...
            } else {
                interactionManager.isAtdistanceToInteract = false;
                NoNegative(speed = Speed[EnemyType.Random] + 4f);
            }
        }
    }
}
