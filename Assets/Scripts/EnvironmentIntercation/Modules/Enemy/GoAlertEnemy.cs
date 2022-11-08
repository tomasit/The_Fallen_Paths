using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using static EnemyInfo;

public class GoAlertEnemy : ACoroutine
{
    private AEnemyMovement movementManager;
    private bool notEndCoroutine = true;
    private Transform aim;
    private Transform self;

    private void Start()
    {
        movementManager = GetComponent<AEnemyMovement>();
        eventType = EnemyEventState.SeenPlayer;
    }

    public override IEnumerator Interact(Transform enemyToAlert = null)
    {
        if (enemyToAlert.gameObject == null) {
            Debug.Log("Enemy to alert, gameobject is null");
            yield return null;
        }

        movementManager.target = enemyToAlert;
        //afficher le pop up houlala y a un player
        Debug.Log(gameObject.name + " running for guard : " + enemyToAlert.gameObject.name);

        var triggerProcessor = enemyToAlert.gameObject.GetComponent<DetectionToSpotEnemy>();
        triggerProcessor.SetDisabling(false);
        triggerProcessor.SetInteractionObj(transform);
        
        aim = enemyToAlert;
        self = transform;
        yield return EndCoroutine();
    }

    // check if corooutine is done
    IEnumerator EndCoroutine() 
    {
        while(notEndCoroutine) {
            yield return WaitUntilTrue(isEnemyAtDistanceToInteract);
            notEndCoroutine = false;
        }
    }

    public bool isEnemyAtDistanceToInteract()
    {
        if (RangeOf(aim.position.x, self.position.x, DistanceToInteract) && 
            RangeOf(aim.position.y, self.position.y, 0.25f)) {
            return true;
        }
        return false;
    }

    public IEnumerator WaitUntilTrue(Func<bool> checkMethod)
    {
        while (checkMethod() == false)
        {
            yield return null;
        }
    }
}
