using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static EnemyInfo;

//laders, stairs ...
public class EnemyInteractions : AEnemyInteraction
{
    [HideInInspector]private EnemyDetectionManager detectionManager;

    void Start()
    {
        IgnoreLayers();
        detectionManager = GetComponent<EnemyDetectionManager>();
    }

    void Update()
    {
        /*if (isAtdistanceToInteract) {
            if (clockCoolDown >= coolDown) {
                guard.GetComponent<EnemyDetectionManager>().SetState(DetectionState.Spoted);
                clockCoolDown = 0f;
                //il l'a deja prevenu
            }
            clockCoolDown += Time.deltaTime;
        } else {
            clockCoolDown = coolDown;
        }*/
    }

    public override void Interact(GameObject obj, ActionType action)
    {
        action = action;
        //si c player et attack tu attack
        //si c guard et alert tu alert
        //si c lader et climb tu climb
    }
}
