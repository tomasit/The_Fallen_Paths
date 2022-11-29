using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//est ce que l'archer detecte le player avec un raycast droit ?
public class EnemyDetectionManager : MonoBehaviour
{
    [Header("Debug")]
    public bool debug = false;
    
    [Header("Raycast")]
    public float detectionDistance = 5f;
    public Vector2 direction = Vector2.right;
    public Vector3 rayCastOffset;

    [Header("States")]
    public bool playerDetected = false;
    [SerializeField] private DetectionState detectionState = DetectionState.None;
    public RaycastHit2D raycast;
    
    [Header("Clocks")]
    public float timeToSpotPlayer = 1.5f;
    public float detectionClock = 0f;
    public float timeToForgetAlerted = 5f;
    public float forgetAlertClock = 0f;
    public float timeToForgetSpoted = 10f;
    public float forgetSpotClock = 0f;

    [Header("Positions")]
    public Vector3 lastEventPosition;

    [Header("Maybe tarsh later idk")]
    public EnemyDialogManager dialogManager;

    void Start()
    {
        dialogManager = GetComponent<EnemyDialogManager>();
    }
    
    void Update()
    {
        playerDetected = ThrowRay(direction, detectionDistance);
        ModifyDetectionState();
    }

    private bool ThrowRay(Vector2 directionRay, float distance)
    {
        UpdateOffsetRaycast();
        RaycastHit2D raycast = Physics2D.Raycast(
            transform.position + rayCastOffset,
            directionRay, 
            float.PositiveInfinity, 
            (1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Wall") | 
            1 << LayerMask.NameToLayer("Enemy") | 1 << LayerMask.NameToLayer("Ground")));

        if (raycast.collider != null)
        {
            if (Vector2.Distance(raycast.point, transform.position + rayCastOffset) <=  distance) {
                distance = raycast.distance;
            }
            DebugRay(debug, distance, directionRay, Color.green);

            if (LayerMask.NameToLayer("Enemy") == raycast.collider.gameObject.layer) {
                if (Vector2.Distance(raycast.point, transform.position + rayCastOffset) <=  distance) {
                    DebugRay(debug, distance, directionRay, Color.blue);
                    //si il est mort : lastEventPosition = raycast.collider.gameObject.transform.position;
                    //Debug.Log("Ca detect les enemy");
                }
            }

            if (LayerMask.NameToLayer("Player") == raycast.collider.gameObject.layer) {
                if (Vector2.Distance(raycast.point, transform.position + rayCastOffset) <=  distance) {
                    //si il est hide dans un truc ne pas le detecter
                    if (!raycast.collider.gameObject.GetComponent<HideInteraction>().IsHide()) {
                        DebugRay(debug, distance, directionRay, Color.red);
                        lastEventPosition = raycast.collider.gameObject.transform.position;
                        return true;
                    }
                }
            }
        } else {
            DebugRay(debug, distance, directionRay, Color.green);
        }
        return false;
    }

    public void UpdateOffsetRaycast()
    {
        if (direction == Vector2.right) {
            rayCastOffset = new Vector3(Mathf.Abs(rayCastOffset.x), rayCastOffset.y, rayCastOffset.z);
        } else if (direction == Vector2.left) {
            rayCastOffset = new Vector3(-Mathf.Abs(rayCastOffset.x), rayCastOffset.y, rayCastOffset.z);
        }
    }

    public void SetRayCastDirection(Vector2 directionToSet)
    {
        direction = directionToSet;
    }

    public void SetState(DetectionState state)
    {
        //Debug.Log("State to assign : " + state + " / Actual state : " + detectionState);
        if (state != detectionState) {
            dialogManager.ChoosDialogType(state);
            detectionState = state;
            var clocks = new [] {detectionClock, forgetAlertClock, forgetSpotClock};
            ResetClocks(ref clocks, 3);
        }
    }

    public DetectionState GetState()
    {
        return detectionState;
    }

    //quand il change de state trigger un dialog etc
    private void ModifyDetectionState()
    {
        //au bout de 2s sur la forgetAlertClcok quand il te playerDetected == false. il va passer en spoted quand meme
        InitStateVariables();

        if (playerDetected) {
            if (detectionState == DetectionState.None || detectionState == DetectionState.Alert) {
                if (DetectionClock(timeToSpotPlayer, ref detectionClock, DetectionState.Spoted)) {
                    var clocks = new [] {detectionClock, forgetAlertClock, forgetSpotClock};
                    ResetClocks(ref clocks, 3);
                }
            }
        } else if (!playerDetected) {
            if (detectionState == DetectionState.Alert) {
                DetectionClock(timeToForgetAlerted, ref forgetAlertClock, DetectionState.None);
            } else if (detectionState == DetectionState.Spoted) {
                DetectionClock(timeToForgetSpoted, ref forgetSpotClock, DetectionState.Alert);
            }
        }
    }

    private void ResetClocks(ref float [] clocks, int size)
    {
        for (int index = 0; index != size; ++index) {
            clocks[index] = 0f;
        }
    }

    private bool DetectionClock(float time, ref float clock, DetectionState stateToAssign) 
    {
        clock += Time.deltaTime;
        if (clock >= time) {
            SetState(stateToAssign);
            return true;
        }
        return false;
    }

    private void InitStateVariables()
    {
        if (!playerDetected) {
            detectionClock = 0f;
        } else if (playerDetected) {
            if (detectionState == DetectionState.Spoted) {
                forgetSpotClock = 0f;
            }
            if (detectionState == DetectionState.None || detectionState == DetectionState.Alert) {
                if (detectionState == DetectionState.None) {
                    SetState(DetectionState.Alert);
                    //detectionState = DetectionState.Alert;
                }
                if (detectionState == DetectionState.Alert) {
                    forgetAlertClock = 0f;
                }
            }
        }
    }

    public void DebugRay(bool draw, float dist, Vector2 direction, Color color)
    {
        if (draw) {
            Debug.DrawRay(transform.position + rayCastOffset, direction * dist, color);
        }
    }
}
