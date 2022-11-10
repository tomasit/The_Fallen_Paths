using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using static EnemyInfo;

public class Flee : ACoroutine
{
    private AEnemyMovement movementManager;
    private EnemyDetectionManager detectionManager;
    private Transform player;
    private bool notEndCoroutine = true;
    private Transform aim;
    private Transform self;
    //private RoomProprieties fleePoints;

    private void Start()
    {
        player = ((PlayerController)FindObjectOfType(typeof(PlayerController))).transform;
        
        detectionManager = GetComponent<EnemyDetectionManager>();
        movementManager = GetComponent<AEnemyMovement>();
        eventType = EnemyEventState.NoGuardAround;
    }

    public override IEnumerator Interact(Transform obj = null)
    {
        detectionManager.detectionState = DetectionState.Flee;

        //Debug.Log("ON ATTEND DE TRIGGER L'ANIM");
        aim = movementManager.target;
        self = transform;
        yield return EndCoroutine();
        //Debug.Log("TRIGGER ANIM T'AS PEUR TAH LES FOU");

        //trigger animation trembler

        yield return null;
    }

    // check if corooutine is done
    IEnumerator EndCoroutine()
    {
        while(notEndCoroutine) {
            yield return WaitUntilTrue(isEnemyReachPoint);
            notEndCoroutine = false;
        }
    }

    public bool isEnemyReachPoint()
    {
        //Debug.Log("name of target point = " + aim.gameObject.name);
        //Debug.Log("aim.position = " + aim.position);
        //Debug.Log("self.position = " + self.position);
        //Debug.Log("----------------------------------");
        if (RangeOf(aim.position.x, self.position.x, 0.5f) && 
            RangeOf(aim.position.y, self.position.y, 0.5f)) {
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
