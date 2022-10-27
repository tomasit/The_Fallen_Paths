using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// COMPORTEMENTS : 

//le manque de lumière emepeche l'enemy de voir

//si le player est en tenue de garde ne rien faire, sinon le detecter

public class EnemyDetectionManager : MonoBehaviour
{
    [Header("Debug")]
    public bool debug = false;
    
    [Header("Raycast")]
    public float detectionDistanceFront = 5f;
    public float detectionDistanceBack = 2f;
    public Vector2 _direction = Vector2.right;
    public Transform colliderTransform;

    [Header("States")]
    public bool playerDetected = false;
    public DetectionState detectionState = DetectionState.None;
    
    [Header("Clocks")]
    public float timeToSpotPlayer = 1.5f;
    public float detectionClock = 0f;
    public float timeToForgetAlerted = 5f;
    public float forgetAlertClock = 0f;
    public float timeToForgetSpoted = 10f;
    public float forgetSpotClock = 0f;

    void Start()
    {
        colliderTransform = transform.GetChild(0).transform;
    }

    void Update()
    {
        //if le mec est pas sneak check derere
        //if le mec est sneak pas check derière

        //si le player est sneak diminuer : distance (detectionDistance)
        if (!ThrowRay(_direction, detectionDistanceFront) && !ThrowRay(-_direction, detectionDistanceBack)) {
            playerDetected = false;
        } else {
            playerDetected = true;
        }

        ModifyDetectionState();
    }

    public void SetRayCastDirection(Vector2 direction)
    {
        _direction = direction;
    }

    public void SetState(DetectionState state)
    {
        detectionState = state;
        var clocks = new [] {detectionClock, forgetAlertClock, forgetSpotClock};
        ResetClocks(ref clocks);
    }

    private void ModifyDetectionState()
    {
        SetStateVariables();

        if (playerDetected) {
            if (detectionState == DetectionState.None || detectionState == DetectionState.Alert) {
                if (DetectionClock(timeToSpotPlayer, ref detectionClock, DetectionState.Spoted)) {
                    var clocks = new [] {detectionClock, forgetAlertClock, forgetSpotClock};
                    ResetClocks(ref clocks);
                }
            }
        } else if (!playerDetected) {
            if (detectionState == DetectionState.Alert) {
                DetectionClock(timeToForgetAlerted, ref forgetAlertClock, DetectionState.None);
            } else if (detectionState == DetectionState.Spoted) {
                DetectionClock(timeToForgetSpoted, ref forgetSpotClock, DetectionState.None);
            }
        }
    }

    private void ResetClocks(ref float [] clocks)
    {
        foreach (int index in clocks) {
            clocks[index] = 0f;
        }
    }

    private bool DetectionClock(float time, ref float clock, DetectionState stateToAssign) 
    {
        clock += Time.deltaTime;
        if (clock >= time) {
            detectionState = stateToAssign;
            return true;
        }
        return false;
    }

    private void DebugRay(bool draw, float dist, Vector2 direction, Color color)
    {
        if (draw) {
            Debug.DrawRay(colliderTransform.position, direction * dist, color);
        }
    }

    private bool ThrowRay(Vector2 directionRay, float distance)
    {
        RaycastHit2D raycastDetection = Physics2D.Raycast(
            colliderTransform.position, 
            directionRay, 
            float.PositiveInfinity, 
            (1 << LayerMask.NameToLayer("Player") | (1 << LayerMask.NameToLayer("Wall"))));

        if (raycastDetection.collider != null)
        {
            if (Vector2.Distance(raycastDetection.point, colliderTransform.position) <=  distance) {
                distance = raycastDetection.distance;
            }
            DebugRay(debug, distance, directionRay, Color.green);

            if (LayerMask.NameToLayer("Player") == raycastDetection.collider.gameObject.layer) {
                if (Vector2.Distance(raycastDetection.point, colliderTransform.position) <=  distance) {
                    DebugRay(debug, distance, directionRay, Color.red);
                    return true;
                }
            }
        }
        return false;
    }

    private void SetStateVariables()
    {
        if (!playerDetected) {
            detectionClock = 0f;
        } else if (playerDetected) {
            if (detectionState == DetectionState.Spoted) {
                forgetSpotClock = 0f;
            }
            if (detectionState == DetectionState.None || detectionState == DetectionState.Alert) {
                if (detectionState == DetectionState.None) {
                    detectionState = DetectionState.Alert;
                }
                if (detectionState == DetectionState.Alert) {
                    forgetAlertClock = 0f;
                }
            }
        }
    }
}
