using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static EnemyInfo;

public abstract class AEnemyMovement : MonoBehaviour
{
    public Transform target;
    public bool isAtDistanceToInteract = false;
    public float speed = 1f;
    protected Transform spritePos;
    [SerializeField] protected Transform _targetPosition;
    [HideInInspector] protected Agent agentMovement;
    [HideInInspector] protected EnemyDetectionManager detectionManager;
    [HideInInspector] protected TriggerCoroutineProcessor detectionTrigger;

    public abstract void BasicMovement();

    public abstract void AlertMovement();

    public abstract void SpotMovement();

    public abstract void FleeMovement();

    public void FreezeMovement()
    {
        target = transform;
    }

    public void Move()
    {
        //if isAtDistanceToInteract => stop

        //if (speed == 0f) {
            //agentMovement.SetTarget(gameObject.transform, transform.position);
        //} else {
            agentMovement.SetTarget(target, detectionManager.rayCastOffset);
        //}
        agentMovement.SetSpeed(speed);
        RotateAxis();
    }

    public void AllowedMovement()
    {
        //gameObject.GetComponent<Rigidbody2D>().freezeRotation = true;
    }

    public float NoNegative(float value)
    {
        if (value < 0) {
            return 0;
        }
        return value;
    }

    private void RotateAxis()
    {
        if (detectionManager.direction == Vector2.right) {
            transform.eulerAngles = new Vector3(0, 0, 0);
        } else if (detectionManager.direction == Vector2.left) {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }
}
