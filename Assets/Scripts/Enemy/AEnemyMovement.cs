using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AEnemyMovement : MonoBehaviour
{
    public Transform target;
    public float speed = 1f;
    [HideInInspector] public Agent agentMovement;
    //[HideInInspector] public GameObject enemy;
    [HideInInspector] public AEnemyInteraction interactionManager;
    [HideInInspector] public EnemyDetectionManager detectionManager;

    public abstract void BasicMovement();

    public abstract void AlertMovement();

    public abstract void SpotMovement();

    public void Move()
    {
        agentMovement.SetTarget(target/*, enemy.transform*/);
        agentMovement.SetSpeed(speed);
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

    public Vector3 FindTargetDirection(Vector3 targetPosition)
    {
        return targetPosition - transform.position;
    }

    //it's not the nearest, just the first he find
    public Vector3 FindNearestEntityDirection(System.Type type)
    {
        GameObject[] allObjects = (GameObject[])Object.FindObjectsOfType(type);
        
        foreach(GameObject obj in allObjects) {
            Vector3 positionObj = obj.transform.position;
            return FindTargetDirection(positionObj);
        }
        Debug.Log("No Entity near");
        return Vector3.zero;
    }
    
    //it's not the nearest, just the first he find
    public (Vector3, GameObject) FindNearestEnemy(System.Type type)
    {
        GameObject[] allObjects = (GameObject[])Object.FindObjectsOfType(type);
        
        foreach(GameObject obj in allObjects) {
            Vector3 positionObj = obj.transform.position;
            return (FindTargetDirection(positionObj), obj);
        }
        Debug.Log("No enemy near");
        return (Vector3.zero, null);
    }
}
