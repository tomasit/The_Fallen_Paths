using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static EnemyInfo;

public class AttackInteraction : AEnemyInteraction
{
    void Start()
    {
        IgnoreLayers();
        clockCoolDown = coolDown;
    }

    void Update()
    {
        if (isAtdistanceToInteract) {
            if (clockCoolDown >= coolDown) {
                Debug.Log("Hit ! of : " + damage);
                clockCoolDown = 0f;
            }
            clockCoolDown += Time.deltaTime;
        } else {
            clockCoolDown = coolDown;
        }
    }

    public override void Interact(GameObject obj, ActionType action)
    {
        //si c player et attack tu attack
        //si c guard et alert tu alert
        //si c lader et climb tu climb
    }
}
