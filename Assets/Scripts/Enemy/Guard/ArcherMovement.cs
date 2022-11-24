using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static EnemyInfo;

public class ArcherMovement : AEnemyMovement
{
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
        if (isClimbing || isEndClimbing) {
            speed = Speed[EnemyType.Archer];
        }
        Move();
        AllowedMovement();
    }
    public override void BasicMovement()
    {
        Vector3 targetDirection = FindTargetDirection(spritePos.position, target.position);

        if (targetDirection.x > 0) {
            if (RangeOf(FindDistanceToAttack(target).x, transform.position.x, 0.1f) && RangeOf(targetDirection.y, 0f, 0.80f)) {
                isAtDistanceToInteract = true;
            } else {
                isAtDistanceToInteract = false;
            }
        }
        if (targetDirection.x < 0) {
            if (RangeOf(FindDistanceToAttack(target).x, transform.position.x, 0.1f) && RangeOf(targetDirection.y, 0f, 0.80f)) {
                isAtDistanceToInteract = true;
            } else {
                isAtDistanceToInteract = false;
            }
        }

        detectionTrigger.SetState(EnemyEventState.None);
        detectionTrigger.SetDisabling(true);
        _targetPosition.localPosition = Vector3.zero;
        NoNegative(speed = Speed[EnemyType.Archer]);
    }

    public override void AlertMovement()
    {
        Vector3 targetDirection = FindTargetDirection(spritePos.position, detectionManager.lastEventPosition);
        _targetPosition.position = detectionManager.lastEventPosition;
        target = _targetPosition;
        
        NoNegative(speed = Speed[EnemyType.Archer]);

        //faudrait quand meme que si le player est sur le collider de l'enemy, le spot direct
        if (targetDirection.x == 0 && RangeOf(targetDirection.y, 0f, 0.80f)) {
            isAtDistanceToInteract = true;
        } else {
            isAtDistanceToInteract = false;
        }
        if (targetDirection.x > 0) {
            if (RangeOf(FindDistanceToAttack(target).x, transform.position.x, 0.1f) && RangeOf(targetDirection.y, 0f, 0.80f)) {
                isAtDistanceToInteract = true;
            } else {
                isAtDistanceToInteract = false;
            }
        }
        if (targetDirection.x < 0) {
            if (RangeOf(FindDistanceToAttack(target).x, transform.position.x, 0.1f) && RangeOf(targetDirection.y, 0f, 0.80f)) {
                isAtDistanceToInteract = true;
            } else {
                isAtDistanceToInteract = false;
            }
        }

        detectionTrigger.SetState(EnemyEventState.None);
        detectionTrigger.SetDisabling(true);
    }

    public override void SpotMovement()
    {
        Debug.Log("Attack pass");

        _targetPosition.localPosition = Vector3.zero;
        Vector3 targetDirection = FindTargetDirection(spritePos.position, player.position);
        target = player;

        Attack(player.transform);
        isAtDistanceToInteract = true;
        speed = 0f;

        /*if (targetDirection.x > 0) {
            if (RangeOf(FindDistanceToAttack(target).x, transform.position.x, 0.1f) && RangeOf(targetDirection.y, 0f, 0.80f)) {
                Attack(player.transform);
                isAtDistanceToInteract = true;
                speed = 0f;
            } else {
                isAtDistanceToInteract = false;
                NoNegative(speed = Speed[EnemyType.Archer] + (Speed[EnemyType.Archer] * 1.5f));
            }
        }
        if (targetDirection.x < 0) {
            if (RangeOf(FindDistanceToAttack(target).x, transform.position.x, 0.1f) && RangeOf(targetDirection.y, 0f, 0.80f)) {
                Attack(player.transform);
                isAtDistanceToInteract = true;
                speed = 0f;
            } else {
                isAtDistanceToInteract = false;
                NoNegative(speed = Speed[EnemyType.Archer] + (Speed[EnemyType.Archer] * 1.5f));
            }
        }*/
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
        NoNegative(speed = Speed[EnemyType.Archer] + (Speed[EnemyType.Archer] * 1.5f));
    }
}
