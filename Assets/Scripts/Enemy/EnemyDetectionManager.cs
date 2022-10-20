using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// COMPORTEMENTS : 
// alerteur : si il detect le player partir vers le sortie
// guard : si il detect le player inscrease sa speed et aller vers lui -> (le suivre peux importe sa room)

//plus le player est lourd il est detecté loin

//le manque de lumière emepeche l'enemy de voir

//si le player est en tenue de garde ne rien faire, sinon le detecter


public class EnemyDetectionManager : MonoBehaviour
{
    [Header("Debug")]
    public bool debug = false;
    
    [Header("Raycast")]
    public float detectionDistanceFront = 5f;
    public float detectionDistanceBack = 2f;
    public Vector2 direction = Vector2.right;

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
        
    }

    void Update()
    {
        if (!ThrowRay(direction, detectionDistanceFront) && !ThrowRay(-direction, detectionDistanceBack)) {
            playerDetected = false;
        } else {
            playerDetected = true;
        }

        //check distance to player

        ModifyDetectionState();
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

    private bool ThrowRay(Vector2 directionRay, float distance)
    {
        RaycastHit2D raycastDetection = Physics2D.Raycast(transform.position, directionRay, float.PositiveInfinity, LayerMask.GetMask("Player"));

        if (debug) {
            Debug.DrawRay(transform.position, directionRay * distance, Color.green);
        }
        
        if (raycastDetection.collider != null)
        {
            if (LayerMask.NameToLayer("Player") == raycastDetection.collider.gameObject.layer) {
                if (Vector2.Distance(raycastDetection.point, transform.position) <=  distance) {
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
