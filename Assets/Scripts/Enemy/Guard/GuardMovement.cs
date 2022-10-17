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
        detectionManager = GetComponent<EnenmyDectionManager>();
    }

    void Update()
    {
        Move();
        AllowedMovement();
    }
    public override void BasicMovement()
    {
        speed = EnemySpeed[EnemyType.Guard];
    }

    public override void AlertMovement()
    {
        Vector3 targetDirection = FindTargetDirection(player);
        
        speed = EnemySpeed[EnemyType.Guard] - 2f;
        if (targetDirection.x > 0) {
            if (targetDirection.x < distanceToAttack) {
                detectionManager.SetState(DetectionState.Spoted);
            }
            Debug.Log("droite : " + targetDirection);
            direction = 1;
        }
        if (targetDirection.x < 0) {
            if (targetDirection.x > -distanceToAttack) {
                detectionManager.SetState(DetectionState.Spoted);
            }
            Debug.Log("gauche : " + targetDirection);
            direction = -1;
        }

        //si le player est en haut chercher l'echelle la plus proche
        //si il est en bas pareil
    }

    public override void SpotMovement()
    {
        Vector3 targetDirection = FindTargetDirection(player);

        if (targetDirection.x > 0) {
            direction = 1;
            if (targetDirection.x < distanceToAttack) {
                isAtdistanceToAttack = true;
                speed = 0f;
            } else {
                isAtdistanceToAttack = false;
                speed = EnemySpeed[EnemyType.Guard] + 2f;
            }
        }
        if (targetDirection.x < 0) {
            direction = -1;
            if (targetDirection.x > -distanceToAttack) {
                isAtdistanceToAttack = true;
                speed = 0f;
            } else {
                isAtdistanceToAttack = false;
                speed = EnemySpeed[EnemyType.Guard] + 2f;
            }
        }
    }
}
