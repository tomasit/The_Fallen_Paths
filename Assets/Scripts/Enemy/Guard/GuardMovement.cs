using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static EnemyInfo;

public class GuardMovement : AEnemyMovement
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
        NoNegative(speed = Speed[EnemyType.Guard]);
    }

    public override void AlertMovement()
    {
        Vector3 targetDirection = FindTargetDirection(player.position);
        
        NoNegative(speed = Speed[EnemyType.Guard] - 2f);
        if (targetDirection.x > 0) {
            if (targetDirection.x < EnemyInfo.DistanceToInteract[EnemyType.Guard]) {
                detectionManager.SetState(DetectionState.Spoted);
            }
            direction = 1;
        }
        if (targetDirection.x < 0) {
            if (targetDirection.x > -EnemyInfo.DistanceToInteract[EnemyType.Guard]) {
                detectionManager.SetState(DetectionState.Spoted);
            }
            direction = -1;
        }

        //si le player est en haut chercher l'echelle la plus proche
        //si il est en bas pareil
    }

    public override void SpotMovement()
    {
        Vector3 targetDirection = FindTargetDirection(player.position);

        if (targetDirection.x > 0) {
            direction = 1;
            if (targetDirection.x < EnemyInfo.DistanceToInteract[EnemyType.Guard]) {// et que le Y est le meme
                interactionManager.isAtdistanceToInteract = true;
                speed = 0f;
            } else {
                interactionManager.isAtdistanceToInteract = false;
                NoNegative(speed = Speed[EnemyType.Guard] + 2f);
            }
        }
        if (targetDirection.x < 0) {
            direction = -1;
            if (targetDirection.x > -EnemyInfo.DistanceToInteract[EnemyType.Guard]) {// et que le Y est le meme
                interactionManager.isAtdistanceToInteract = true;
                speed = 0f;
            } else {
                interactionManager.isAtdistanceToInteract = false;
                NoNegative(speed = Speed[EnemyType.Guard] + 2f);
            }
        }
    }
}
