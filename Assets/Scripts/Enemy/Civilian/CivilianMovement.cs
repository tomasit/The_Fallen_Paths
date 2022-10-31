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
        detectionTrigger = GetComponent<TriggerCoroutineProcessor>();
    }

    void Update()
    {
        Move();
        AllowedMovement();
    }
    public override void BasicMovement()
    {
        detectionTrigger.SetDisabling(true);

        NoNegative(speed = Speed[EnemyType.Random]);
    }

    public override void AlertMovement()
    {
        detectionTrigger.SetDisabling(true);

        Vector3 targetDirection = FindTargetDirection(player.position);
        target = player;
        
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
        detectionTrigger.SetState(EnemyEventState.SeenPlayer);
        detectionTrigger.SetDisabling(false);
        NoNegative(speed = Speed[EnemyType.Random] + (Speed[EnemyType.Random] * 1.5f));

        //Il faut pas que ce soit le AenemyMovement qui lance le trigger 
        // mais que ce soit le detectionManager
    }
}
