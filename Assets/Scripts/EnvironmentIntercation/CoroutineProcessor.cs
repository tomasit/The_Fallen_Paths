using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum EnemyEventState {
    None,
    SeenPlayer,
    NoGuardAround,
    SeenRandomSpoted,
    SeenDeadBody,
    SeenOffLight,
}

[RequireComponent(typeof(TriggerCoroutineProcessor))]
public class CoroutineProcessor : ACoroutine
{
    public bool crRunning = false;
    public EnemyEventState enemyState = EnemyEventState.None;
    public List<GameObject> enemiesDiscovered;
    [SerializeField] private ACoroutine[] _coroutines;
    [SerializeField] private bool _execAllCoroutines = false;
    [SerializeField] private bool _stopOnInteract;
    [HideInInspector] public bool _enabled = true;
    private bool _interact = false;
    private TriggerProcessor _triggerInteractor = null;

    private void Start()
    {
        crRunning = false;
        _triggerInteractor = GetComponent<TriggerProcessor>();
        if (_interact)
            DisableTriggerInteractor();
    }

    private void DisableTriggerInteractor()
    {
        _triggerInteractor.DisableTrigger();
    }

    public override IEnumerator Interact(Transform objArg = null)
    {
        crRunning = true;
        if (_interact || !_enabled)
            yield return null;

        foreach (var coroutine in _coroutines)
        {
            if (_execAllCoroutines) {
                StartCoroutine(coroutine.Interact(objArg));
            } else {
                if (enemyState == EnemyEventState.None) {
                    yield return StartCoroutine(coroutine.Interact(objArg));
                }
                if (enemyState == EnemyEventState.SeenPlayer) {
                    var enemy = FindNearestEnemy();
                    if (enemy != null) {
                        enemiesDiscovered.Add(enemy.gameObject);
                        yield return StartCoroutine(coroutine.Interact(enemy));
                    } else {
                        enemyState = EnemyEventState.NoGuardAround;
                        enemiesDiscovered.Clear();
                    }
                }
                if (enemyState == EnemyEventState.NoGuardAround) {
                    Debug.Log("------------random is going to do run away");
                    //run oposite dir than player
                    yield return StartCoroutine(coroutine.Interact());
                }
                if (enemyState == EnemyEventState.SeenRandomSpoted) {
                    Debug.Log("------------guard is going to hagar le PLAYER");
                    //go to last player pos
                    yield return StartCoroutine(coroutine.Interact(objArg));
                }
                if (enemyState == EnemyEventState.SeenDeadBody) {
                    //be alerted
                    yield return StartCoroutine(coroutine.Interact());
                }
                if (enemyState == EnemyEventState.SeenOffLight) {
                    //turn on the light
                    yield return StartCoroutine(coroutine.Interact(objArg));                    
                }
            }
        }

        if (_stopOnInteract) {
            _interact = true;
            DisableTriggerInteractor();
        }
        crRunning = false;
    }
    
    public bool enemyAlreadyDiscovered(GameObject enemyFound)
    {
        foreach (var enemy in enemiesDiscovered) {
            if (enemy == enemyFound) {
                return true;
            }
        }
        return false;
    }

    public Transform FindNearestEnemy()
    {
        GuardMovement[] allObjects = (GuardMovement[])GameObject.FindObjectsOfType(typeof(GuardMovement));

        (Vector3 pos, GameObject obj) enemySaved = (allObjects[0].gameObject.transform.position, allObjects[0].gameObject);
        bool newEnemyFound = false;

        foreach(GuardMovement obj in allObjects) {
            Vector3 enemyPosIte = ComparePosition(obj.gameObject.transform.position, transform.position);
            
            if (enemyAlreadyDiscovered(obj.gameObject) || 
                obj.gameObject.GetComponent<EnemyDetectionManager>().detectionState == DetectionState.Spoted) {
                continue;
            }

            if (Mathf.Abs(enemyPosIte.y) < Mathf.Abs(enemySaved.pos.y)) {
                if (Mathf.Abs(enemyPosIte.y) < Mathf.Abs(enemySaved.pos.x)) {
                    enemySaved = (enemyPosIte, obj.gameObject);
                    newEnemyFound = true;
                }
            }
            if (Mathf.Abs(enemyPosIte.x) < Mathf.Abs(enemySaved.pos.x)) {
                enemySaved = (enemyPosIte, obj.gameObject);
                newEnemyFound = true;
            }
        }

        return newEnemyFound ? enemySaved.obj.gameObject.transform : null;
    }

    public Vector3 ComparePosition(Vector3 targetPosition, Vector3 actualPosition)
    {
        return targetPosition - actualPosition;
    }
}