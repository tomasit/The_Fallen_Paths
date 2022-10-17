using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AEnemyMovement : MonoBehaviour
{
    public int direction = 1; // right : 1, left : -1
    public float speed = 1f;
    public float distanceToAttack = 2f;
    public bool isAtdistanceToAttack = false;
    [HideInInspector]public EnenmyDectionManager detectionManager;

    public void Move()
    {
        Vector3 movement = new Vector3(speed * direction, 0, 0);

        movement *= Time.deltaTime;
        //Debug.Log("Movement : " + movement);

        if (movement == Vector3.zero) {
            return;
        }

        transform.Translate(movement);
    }

    public void AllowedMovement() 
    {
        gameObject.GetComponent<Rigidbody2D>().freezeRotation = true;
    }

    public Vector3 FindTargetDirection(Transform target)
    {
        return target.position - transform.position;
    }

    public abstract void BasicMovement();

    public abstract void AlertMovement();

    public abstract void SpotMovement();
}
