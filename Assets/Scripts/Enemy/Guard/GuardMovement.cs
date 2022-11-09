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
        agentMovement = GetComponent<Agent>();
        detectionTrigger = GetComponent<TriggerCoroutineProcessor>();
        spritePos = transform.GetChild(0).GetComponent<SpriteRenderer>().transform;
    }

    void Update()
    {
        Move();
        AllowedMovement();

        //Spot ou Alert : si t a la position de destinationfaire des gauche droite de direction de la tête
    }
    public override void BasicMovement()
    {
        Vector3 targetDirection = FindTargetDirection(spritePos.position, target.position);

        if (targetDirection.x > 0) {
            if (targetDirection.x < EnemyInfo.DistanceToInteract && RangeOf(targetDirection.y, 0f, 0.80f)) {
                isAtDistanceToInteract = true;
            } else {
                isAtDistanceToInteract = false;
            }
        }
        if (targetDirection.x < 0) {
            if (targetDirection.x > -EnemyInfo.DistanceToInteract && RangeOf(targetDirection.y, 0f, 0.80f)) {
                isAtDistanceToInteract = true;
            } else {
                isAtDistanceToInteract = false;
            }
        }

        detectionTrigger.SetState(EnemyEventState.None);
        detectionTrigger.SetDisabling(true);
        _targetPosition.localPosition = Vector3.zero;
        NoNegative(speed = Speed[EnemyType.Guard]);
    }

    public override void AlertMovement()
    {
        Vector3 targetDirection = FindTargetDirection(spritePos.position, detectionManager.lastEventPosition);
        _targetPosition.position = detectionManager.lastEventPosition;
        target = _targetPosition;
        
        NoNegative(speed = Speed[EnemyType.Guard]);
        if (targetDirection.x > 0) {
            if (targetDirection.x < EnemyInfo.DistanceToInteract && RangeOf(targetDirection.y, 0f, 0.80f)) {
                isAtDistanceToInteract = true;
                detectionManager.SetState(DetectionState.Spoted);
            } else {
                isAtDistanceToInteract = false;
            }
        }
        if (targetDirection.x < 0) {
            if (targetDirection.x > -EnemyInfo.DistanceToInteract && RangeOf(targetDirection.y, 0f, 0.80f)) {
                isAtDistanceToInteract = true;
                detectionManager.SetState(DetectionState.Spoted);
            } else {
                isAtDistanceToInteract = false;
            }
        }

        detectionTrigger.SetState(EnemyEventState.None);
        detectionTrigger.SetDisabling(true);
    }

    public override void SpotMovement()
    {
        _targetPosition.localPosition = Vector3.zero;
        Vector3 targetDirection = FindTargetDirection(spritePos.position, player.position);
        target = player;

        if (targetDirection.x > 0) {
            if (targetDirection.x < EnemyInfo.DistanceToInteract && RangeOf(targetDirection.y, 0f, 0.80f)) {
                
                //il faut pas qu'il soit sur une echelle
                Attack(player.transform);
                
                isAtDistanceToInteract = true;
                speed = 0f;
            } else {
                isAtDistanceToInteract = false;
                NoNegative(speed = Speed[EnemyType.Guard] + (Speed[EnemyType.Guard] * 1.5f));
            }
        }
        if (targetDirection.x < 0) {
            if (targetDirection.x > -EnemyInfo.DistanceToInteract && RangeOf(targetDirection.y, 0f, 0.80f)) {
                Attack(player.transform);
                isAtDistanceToInteract = true;
                speed = 0f;
            } else {
                isAtDistanceToInteract = false;
                NoNegative(speed = Speed[EnemyType.Guard] + (Speed[EnemyType.Guard] * 1.5f));
            }
        }
    }

    private void Attack(Transform objToAttack)
    {
        detectionTrigger.SetState(EnemyEventState.FightPlayer);
        detectionTrigger.SetInteractionObj(objToAttack);
        detectionTrigger.SetDisabling(false);
    }

    public override void FleeMovement()
    {
        _targetPosition.localPosition = Vector3.zero;
        NoNegative(speed = Speed[EnemyType.Guard] + (Speed[EnemyType.Guard] * 1.5f));
    }
}
