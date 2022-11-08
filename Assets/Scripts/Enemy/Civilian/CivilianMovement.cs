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
        agentMovement = GetComponent<Agent>();
        detectionTrigger = GetComponent<TriggerCoroutineProcessor>();
        spritePos = transform.GetChild(0).GetComponent<SpriteRenderer>().transform;
    }

    void Update()
    {
        Move();
        AllowedMovement();
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

        _targetPosition.localPosition = Vector3.zero;
        detectionTrigger.SetDisabling(true);
        NoNegative(speed = Speed[EnemyType.Random]);
    }

    public override void AlertMovement()
    {
        detectionTrigger.SetDisabling(true);
        Vector3 targetDirection = FindTargetDirection(spritePos.position, detectionManager.lastEventPosition);
        _targetPosition.position = detectionManager.lastEventPosition;
        target = _targetPosition;
        
        NoNegative(speed = Speed[EnemyType.Random] - (Speed[EnemyType.Random] * 0.5f));
        if (targetDirection.x > 0) {
            if (targetDirection.x < DistanceToInteract &&
                RangeOf(targetDirection.y, 0f, 0.80f)) {
                detectionManager.SetState(DetectionState.Spoted);
            }
        }
        if (targetDirection.x < 0) {
            if (targetDirection.x > -DistanceToInteract &&
                RangeOf(targetDirection.y, 0f, 0.80f)) {
                detectionManager.SetState(DetectionState.Spoted);
            }
        }
    }

    public override void SpotMovement()
    {
        _targetPosition.localPosition = Vector3.zero;
        detectionTrigger.SetState(EnemyEventState.SeenPlayer);
        detectionTrigger.SetDisabling(false);
        NoNegative(speed = Speed[EnemyType.Random] + (Speed[EnemyType.Random] * 1.5f));
    }

    public override void FleeMovement()
    {
        _targetPosition.localPosition = Vector3.zero;
        //detectionTrigger.SetState(EnemyEventState.NoGuardAround);
        //detectionTrigger.SetDisabling(false);
        NoNegative(speed = Speed[EnemyType.Guard] + (Speed[EnemyType.Guard] * 1.5f));
    }
}
