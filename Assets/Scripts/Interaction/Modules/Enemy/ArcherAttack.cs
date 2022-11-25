using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static EnemyInfo;

public class ArcherAttack : ACoroutine
{
    private TriggerCoroutineProcessor triggerProcessor;
    private AEnemyMovement movementManager;
    private EnemyDetectionManager detectionManager;
    private SpriteRenderer sprite;
    private Animator animator;
    [SerializeField] private GameObject arrow;

    private void Start()
    {
        animator = transform.GetChild(0).gameObject.GetComponent<Animator>();
        sprite = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        triggerProcessor = GetComponent<TriggerCoroutineProcessor>();
        movementManager = GetComponent<AEnemyMovement>();
        detectionManager = GetComponent<EnemyDetectionManager>();
        eventType = EnemyEventState.FightPlayer;
    }

    public override IEnumerator Interact(Transform target = null)
    {
        if ((movementManager.isClimbing || movementManager.isEndClimbing) /* && qu'il se fait pas hit*/) {
            yield return null;
        }

        Vector3 direction = FindTargetDirection(sprite.transform.position, target.position);
        int factorDirection_x = (gameObject.transform.eulerAngles.y == 0 ? 1 : -1);
        int factorDirection_y = 0;
        float valueToAddY = 0f;

        if (direction.y > 1.3f) {
            animator.SetInteger("AttackState", 1);//up
            factorDirection_y = 1;
            valueToAddY = detectionManager.rayCastOffset.y + 1f;
        } else if (direction.y < -0.7f) {
            animator.SetInteger("AttackState", 2);//down
            valueToAddY = detectionManager.rayCastOffset.y;
            factorDirection_y = -1;
        } else {
            animator.SetInteger("AttackState", 0);//down
            factorDirection_y = 1;
            valueToAddY = detectionManager.rayCastOffset.y;
        }

        // SPAWN POSITION
        Vector3 spawnPosition = new Vector3(
            transform.position.x + detectionManager.rayCastOffset.x + (factorDirection_x * arrow.GetComponent<SpriteRenderer>().bounds.size.x), 
            transform.position.y + valueToAddY * factorDirection_y, 
            transform.position.z);
        // INSTANCIATE
        GameObject newArrow = Object.Instantiate(arrow, spawnPosition, transform.rotation);
        // SCALE
        newArrow.transform.localScale = new Vector3(4f, 4f, 1f);
        // ROTATION
        Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90) * direction;
        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);
        newArrow.transform.rotation = targetRotation;

        animator.SetTrigger("Attack");
        
        yield return new WaitForSeconds(4f);//cooldown
        triggerProcessor.SetDisabling(true);
        triggerProcessor.SetCoroutine(null);
    }
}
