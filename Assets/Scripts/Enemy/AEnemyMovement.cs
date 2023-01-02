using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static EnemyInfo;

public abstract class AEnemyMovement : MonoBehaviour
{
    public Transform target;
    [SerializeField] protected Transform _targetPosition;
    public bool isAtDistanceToInteract = false;
    [HideInInspector] public float speed = 1f;
    public float speedFactor = 1f;
    private Vector3 _lastFramePosition;
    private int _hasMoved = 0;

    public Transform collisionObj = null;//pour moi ca sert a rien
    public bool isClimbing = false;
    public bool isEndClimbing = false;

    public bool saveFacingPlayer = false;
    public bool wasAtDistance = false;
    public bool isFacingPlayer = false;
    
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
        agentMovement.SetSpeed(speed * speedFactor);
        CheckMovement();
    }

    public void Rotate()
    {
        IsFacingTarget();

        if (HasMovedFromLastFrame()) {
            //quand ils sont a distance de te tapper && qu'ils sont en face de toi : il ne rotate pas
            if (wasAtDistance && isFacingPlayer)
                return;
            if (DirectionMovedFromLastFrame() < 0)
                transform.eulerAngles = new Vector3(0, 180, 0);
            else
                transform.eulerAngles = new Vector3(0, 0, 0);
        } else {
            //si jamais il bouge pas && qu'il est pas spot : il va pas rotate en fonction du player
            if (detectionManager.GetState() != DetectionState.Spoted)
                return;
            if (detectionManager.direction.x < 0)
                transform.eulerAngles = new Vector3(0, 180, 0);
            else if (detectionManager.direction.x > 0)
                transform.eulerAngles = new Vector3(0, 0, 0);
        }
    }

    private void IsFacingTarget()
    {
        if (detectionManager.playerDetected) {
            Vector3 targetDistance = FindTargetDirection(spritePos.position, target.position);

            if (isAtDistanceToInteract)
                wasAtDistance = true;
            if (transform.eulerAngles.y == 0) {
                if (targetDistance.x > 0) {
                    isFacingPlayer = true;
                } else if (targetDistance.x < 0) {
                    isFacingPlayer = false;
                }
            } else if (transform.eulerAngles.y == 180) {
                if (targetDistance.x > 0) {
                    isFacingPlayer = false;
                } else if (targetDistance.x < 0) {
                    isFacingPlayer = true;
                }
            }
            //si la target a changé de coté
            if (saveFacingPlayer != isFacingPlayer) {
                wasAtDistance = false;
            }
            saveFacingPlayer = isFacingPlayer;
        } else {
            saveFacingPlayer = false;
            wasAtDistance = false;
            isFacingPlayer = false;
        }
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

    //crois que il vont tous a la même vitesse quand ils climb
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
