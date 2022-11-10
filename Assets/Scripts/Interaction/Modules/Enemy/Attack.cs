using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static EnemyInfo;

public class Attack : ACoroutine
{
    private TriggerCoroutineProcessor triggerProcessor;
    private AEnemyMovement movementManager;
    private Animator animator;

    private void Start()
    {
        animator = transform.GetChild(0).gameObject.GetComponent<Animator>();
        triggerProcessor = GetComponent<TriggerCoroutineProcessor>();
        movementManager = GetComponent<AEnemyMovement>();
        eventType = EnemyEventState.FightPlayer;
    }

    public override IEnumerator Interact(Transform obj = null)
    {
        if ((movementManager.isClimbing || movementManager.isEndClimbing)
            /*qu'il se fait pas hit*/) {
            yield return null;
        }
        //Debug.Log("Attack !!!");
        obj.gameObject.GetComponent<BasicHealthWrapper>().Hit(1);//damage
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(2f);//cooldown
        triggerProcessor.SetDisabling(true);
    }
}
