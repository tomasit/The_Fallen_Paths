using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AEnemyMovement : MonoBehaviour
{
    // NOTE : right : 1, left : -1
    public int direction = 1;
    public float speed = 1f;
    [HideInInspector]public AEnemyInteraction interactionManager;
    [HideInInspector]public EnemyDetectionManager detectionManager;

    public abstract void BasicMovement();

    public abstract void AlertMovement();

    public abstract void SpotMovement();

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
