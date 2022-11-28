using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//[RequireComponent(typeof(TriggerCoroutineProcessor))]
public class CoroutineProcessor : ACoroutine
{
    public bool crRunning = false;
    public EnemyEventState enemyState = EnemyEventState.None;
    //public List<GameObject> enemiesDiscovered;
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
                if (enemyState == EnemyEventState.None && coroutine.eventType == EnemyEventState.None) {
                    yield return coroutine.Interact(objArg);
                }
                if (enemyState == EnemyEventState.SeenPlayer && coroutine.eventType == EnemyEventState.SeenPlayer) {
                    //Debug.Log("------------random is going to alert guard");
                    var enemy = FindNearestEnemy();
                    if (enemy != null) {
                        yield return coroutine.Interact(enemy);
                    } else {
                        //Debug.Log("No enemy found, new state for flee");
                        enemyState = EnemyEventState.NoGuardAround;
                    }
                }
                if (enemyState == EnemyEventState.NoGuardAround && coroutine.eventType == EnemyEventState.NoGuardAround) {
                    //Debug.Log("------------random is going to do run away");
                    yield return coroutine.Interact();
                }
                if (enemyState == EnemyEventState.FightPlayer && coroutine.eventType == EnemyEventState.FightPlayer) {
                    //Debug.Log("------------guard is going to hagar le PLAYER");
                    yield return coroutine.Interact(objArg);
                }
                if (enemyState == EnemyEventState.SeenRandomSpoted && coroutine.eventType == EnemyEventState.SeenRandomSpoted) {
                    //Debug.Log("------------guard is running for hagar le PLAYER");
                    yield return coroutine.Interact(objArg);
                }
                // implem ca aussi
                if (enemyState == EnemyEventState.SeenDeadBody && coroutine.eventType == EnemyEventState.SeenDeadBody) {
                    //be alerted
                    yield return coroutine.Interact();
                }
                if (enemyState == EnemyEventState.SeenOffLight && coroutine.eventType == EnemyEventState.SeenOffLight) {
                    //turn on the light
                    yield return coroutine.Interact(objArg);                    
                }
                //si le player peut faire du bruit aussi implem
            }
        }

        if (_stopOnInteract) {
            _interact = true;
            DisableTriggerInteractor();
        }
        enemyState = EnemyEventState.None;
        crRunning = false;
    }
    
    /*public bool enemyAlreadyDiscovered(GameObject enemyFound)
    {
        foreach (var enemy in enemiesDiscovered) {
            if (enemy == enemyFound) {
                return true;
            }
        }
        return false;
    }*/

    public Transform FindNearestEnemy()
    {
        GuardMovement[] allObjects = (GuardMovement[])GameObject.FindObjectsOfType(typeof(GuardMovement));
        if (allObjects.Length == 0)
            return null;
        (Vector3 pos, GameObject obj) enemySaved = (allObjects[0].gameObject.transform.position, allObjects[0].gameObject);
        bool newEnemyFound = false;

        foreach(GuardMovement obj in allObjects) {
            //Debug.Log("Guard remaining : " + obj.gameObject.name);
            Vector3 enemyPosIte = ComparePosition(obj.gameObject.transform.position, transform.position);
            
            if (obj.gameObject.GetComponent<EnemyDetectionManager>().GetState() == DetectionState.Spoted ||
                obj.gameObject.GetComponent<EnemyDetectionManager>().GetState() == DetectionState.Alert) {
                //Debug.Log(obj.gameObject.name + " is knowed or arleady alerted");
                continue;
            }

            if (!newEnemyFound) {
                enemySaved = (enemyPosIte, obj.gameObject);
            }

            newEnemyFound = true;

            if (Mathf.Abs(enemyPosIte.y) < Mathf.Abs(enemySaved.pos.y)) {
                if (Mathf.Abs(enemyPosIte.y) < Mathf.Abs(enemySaved.pos.x)) {
                    enemySaved = (enemyPosIte, obj.gameObject);
                }
            }

            if (Mathf.Abs(enemyPosIte.x) < Mathf.Abs(enemySaved.pos.x)) {
                enemySaved = (enemyPosIte, obj.gameObject);
            }
        }

        //Debug.Log("NewEnemyFound : " + newEnemyFound);

        return newEnemyFound ? enemySaved.obj.gameObject.transform : null;
    }

    public Vector3 ComparePosition(Vector3 targetPosition, Vector3 actualPosition)
    {
        return targetPosition - actualPosition;
    }
}