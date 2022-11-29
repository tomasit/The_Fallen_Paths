using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static EnemyInfo;

public class GoToLastPosition : ACoroutine
{
    private AEnemyMovement movementManager;

    private void Start()
    {
        movementManager = GetComponent<AEnemyMovement>();
        eventType = EnemyEventState.SeenRandomSpoted;
    }

    public override IEnumerator Interact(Transform enemyRandom = null)
    {
        if (enemyRandom == null) {
            yield return null;
        }
        Vector3 positionToGo = enemyRandom.gameObject.GetComponent<EnemyDetectionManager>().lastEventPosition;
        
        GetComponent<EnemyDetectionManager>().lastEventPosition = positionToGo;
        GetComponent<EnemyDetectionManager>().SetState(DetectionState.Alert);

        yield return null;
    }
}
