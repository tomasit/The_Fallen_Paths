using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static EnemyInfo;

public class MageAttack : ACoroutine
{
    private TriggerCoroutineProcessor triggerProcessor;
    private AEnemyMovement movementManager;
    private EnemyDetectionManager detectionManager;
    private SpriteRenderer sprite;
    private Animator animator;
    [SerializeField] private GameObject shootBall;
    [SerializeField] private Vector3 shootObjScale = new Vector3(1f, 1f, 1f);

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
        float valueToAddY = detectionManager.rayCastOffset.y;

        animator.SetInteger("AttackState", 0);//straight
        //animator.SetInteger("AttackState", 1);//troubillons
        //animator.SetInteger("AttackState", 2);//levé septre haut

        // SPAWN POSITION
        Vector3 spawnPosition = new Vector3(
            transform.position.x + detectionManager.rayCastOffset.x, 
            transform.position.y + valueToAddY, 
            transform.position.z);
        // INSTANCIATE
        GameObject newShootBall = Object.Instantiate(shootBall, spawnPosition, transform.rotation, transform);
        // SCALE
        newShootBall.transform.localScale = new Vector3(shootObjScale.x, shootObjScale.y, shootObjScale.z);
        // ROTATION
        Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90) * direction;
        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);
        newShootBall.transform.rotation = targetRotation;

        animator.SetTrigger("Attack");
        
        yield return new WaitForSeconds(4f);//cooldown
        triggerProcessor.Disable(true);
        triggerProcessor.SetCoroutine(null);
    }
}
