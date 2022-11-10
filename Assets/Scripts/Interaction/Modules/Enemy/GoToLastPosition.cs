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
            Debug.Log("Enemy random to check, gameobject is null");
            yield return null;
        }
        Vector3 positionToGo = enemyRandom.gameObject.GetComponent<EnemyDetectionManager>().lastEventPosition;
        
        Debug.Log(gameObject.name + " running last pos : " + positionToGo);
        //afficher le pop up houlala y a un player

        GetComponent<EnemyDetectionManager>().lastEventPosition = positionToGo;
        GetComponent<EnemyDetectionManager>().SetState(DetectionState.Alert);

        yield return null;
    }
}
