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

    public Transform collisionObj = null;
    public bool isClimbing = false;

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

    //si il bouge pas sur une echelle stop la vitesse de l'animation
    //si il est DetectionState == Spoted acceler la vitesse de l'animation de stand_up

    //lader animations triggers
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null) {
            return;
        }
        Vector3 targetDirection = FindTargetDirection(spritePos.position, target.position);
        ///
        if (other.gameObject.layer == LayerMask.NameToLayer("Lader") &&
            !RangeOf(targetDirection.y, 0f, 1f)) {//sa target est pas a son niveau en Y
            
            collisionObj = other.gameObject.transform;
            transform.GetChild(0).GetComponent<Animator>().SetBool("Climbing", true);
        }
        ///
        if (other.gameObject.layer == LayerMask.NameToLayer("LastLader") && collisionObj != null) {
            
            collisionObj = null;
            transform.GetChild(0).GetComponent<Animator>().SetTrigger("StandUp");
            transform.GetChild(0).GetComponent<Animator>().SetBool("Climbing", false);
        }
        ////
        /*if (other.gameObject.layer == LayerMask.NameToLayer("LastLader") && 
            collisionObj == null &&
            targetDirection.y < -1f) {//sa target est en dessous
            Debug.Log("SitDown Bitch !!! / layer = " + other.gameObject.layer + " collisionObj == null ?" + (collisionObj == null) + " targetDirection.y = " + targetDirection.y);
            transform.GetChild(0).GetComponent<Animator>().SetTrigger("SitDown");
        }*/
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other == null) {
            return;
        }
        ///
        if (other.gameObject.layer == LayerMask.NameToLayer("Lader")) {
            transform.GetChild(0).GetComponent<Animator>().SetBool("Climbing", false);
            collisionObj = null;
        }
    }
}
