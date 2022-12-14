using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static EnemyInfo;

public abstract class AEnemyMovement : MonoBehaviour
{
    public Transform target;
    [SerializeField] protected Transform _targetPosition;
    public bool isAtDistanceToInteract = false;
    public float speed = 1f;
    private Vector3 _lastFramePosition;
    private int _hasMoved = 0;

    public Transform collisionObj = null;//pour moi ca sert a rien
    public bool isClimbing = false;
    public bool isEndClimbing = false;
    
    protected Transform player;

    [HideInInspector] public Transform spritePos;
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

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void Move()
    {
        agentMovement.SetTarget(target, detectionManager.rayCastOffset);
        agentMovement.SetSpeed(speed);
        CheckMovement();
    }
    
    private void CheckMovement()
    {
        if (_lastFramePosition == transform.position) {
            _hasMoved = 0;
        } else {
            if (_lastFramePosition.x > transform.position.x) {
                _hasMoved = -1;
            } else {
                _hasMoved = 1;
            }
        }
        _lastFramePosition = transform.position;
    }

    public float NoNegative(float value)
    {
        if (value < 0) {
            return 0;
        }
        return value;
    }

    protected Vector3 FindDistanceToAttack(Transform t)
    {
        Vector3 targetDirection = FindTargetDirection(spritePos.position, t.position);
            
        Vector3 distanceToPlayer = new Vector3(
            t.position.x + (DistanceToInteract.x * (targetDirection.x > 0 ? -1 : 1)), 
            t.position.y + (DistanceToInteract.y * (targetDirection.y > 0 ? 1 : -1)), 
            t.position.z);
        return distanceToPlayer - new Vector3(0f, detectionManager.rayCastOffset.y, 0f);
    }

    public bool HasMovedFromLastFrame()
    {
        return (_hasMoved == 0) ? false : true;
    }

    public int DirectionMovedFromLastFrame()
    {
        return _hasMoved;
    }

    public void AllowedMovement()
    {
        //gameObject.GetComponent<Rigidbody2D>().freezeRotation = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null)
            return;
        Vector3 targetDirection = FindTargetDirection(spritePos.position, target.position);
        
        if (other.gameObject.layer == LayerMask.NameToLayer("Lader") && 
        !RangeOf(targetDirection.y, 0f, 1f)) {
                isClimbing = true;
                collisionObj = other.gameObject.transform;
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("LastLader")) {
            isEndClimbing = true;
            collisionObj = other.gameObject.transform;
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other == null) {
            return;
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("Lader")) {
            collisionObj = null;
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("LastLader")) {
            isEndClimbing = false;
        }
    }
}
