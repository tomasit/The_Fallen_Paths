using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static EnemyInfo;

public class GoAlertEnemy : ACoroutine
{
    private AEnemyMovement movementManager;

    private void Start()
    {
        movementManager = GetComponent<AEnemyMovement>();
    }

    public override IEnumerator Interact(Transform enemyToAlert)
    {
        yield return new WaitForSeconds(1f);

        movementManager.target = enemyToAlert;
        //afficher le pop up houlala y a un player
        Debug.Log(gameObject.name + " running for guard : " + enemyToAlert.gameObject.name);
        
        if (RangeOf(enemyToAlert.transform.position.x, transform.position.x, DistanceToInteract)) {
            Debug.Log("on est dans la range");
            var targetProcessor = enemyToAlert.gameObject.GetComponent<CoroutineProcessor>();
            targetProcessor.enemyState = EnemyEventState.SeenRandomSpoted;
            
            //Transform tmp = new Transform(tmp.position);
            //tmp.position = GetComponent<EnemyDetectionManager>().lastEventPosition;
            yield return targetProcessor.Interact(/*tmp*/);

            //si on fait ca il va suivre le player
            // ?? enemyToAlert.GetComponent<EnemyDetectionManager>().detectionState = DetectionState.Spoted;
        }
    }
}
