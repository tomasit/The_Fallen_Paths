using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static EnemyInfo;

public class Attack : ACoroutine
{
    private TriggerCoroutineProcessor triggerProcessor;
    //l'animator est en dessous
    private Animator animator;

    private void Start()
    {
        animator = transform.GetChild(0).gameObject.GetComponent<Animator>();
        triggerProcessor = GetComponent<TriggerCoroutineProcessor>();
        eventType = EnemyEventState.FightPlayer;
    }

    public override IEnumerator Interact(Transform obj = null)
    {
        //Debug.Log("----Attack");
        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(2f);

        triggerProcessor.SetDisabling(true);
    }
}
