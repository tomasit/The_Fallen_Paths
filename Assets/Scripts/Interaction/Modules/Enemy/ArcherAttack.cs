using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static EnemyInfo;

public class ArcherAttack : ACoroutine
{
    private TriggerCoroutineProcessor triggerProcessor;
    private AEnemyMovement movementManager;
    private EnemyDetectionManager detectionManager;
    private Animator animator;
    [SerializeField] private GameObject arrow;

    private void Start()
    {
        animator = transform.GetChild(0).gameObject.GetComponent<Animator>();
        triggerProcessor = GetComponent<TriggerCoroutineProcessor>();
        movementManager = GetComponent<AEnemyMovement>();
        detectionManager = GetComponent<EnemyDetectionManager>();
        eventType = EnemyEventState.FightPlayer;
    }

    public override IEnumerator Interact(Transform obj = null)
    {
        if ((movementManager.isClimbing || movementManager.isEndClimbing)
            /*qu'il se fait pas hit*/) {
            yield return null;
        }
        
        Vector3 spawnPosition = transform.position + detectionManager.rayCastOffset;
        Vector3 direction = new Vector3(detectionManager.direction.x, detectionManager.direction.y, 0);
        Quaternion rotation = Quaternion.LookRotation(transform.position, direction);

        //Object.Instantiate(arrow, spawnPosition, rotation, transform);
        Object.Instantiate(arrow, spawnPosition, transform.rotation, transform);
        
        //en fonction du Y ou il tire, faire une animation diff
        //animator.SetTrigger("Attack");
        
        yield return new WaitForSeconds(2f);//cooldown
        triggerProcessor.SetDisabling(true);
    }
}
