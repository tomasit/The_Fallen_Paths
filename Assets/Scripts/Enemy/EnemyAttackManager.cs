using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackManager : MonoBehaviour
{
    public float coolDown = 1f;
    public float clockCoolDown = 0f;
    public float damage = 1f;

    private AEnemyMovement movementManager;
    
    void Start()
    {
        movementManager = GetComponent<AEnemyMovement>();
        clockCoolDown = coolDown;
    }

    void Update()
    {
        if (movementManager.isAtdistanceToAttack) {
            if (clockCoolDown >= coolDown) {
                Debug.Log("Hit !");
                clockCoolDown = 0f;
            }
            clockCoolDown += Time.deltaTime;
        } else {
            clockCoolDown = coolDown;
        }
    }
}
