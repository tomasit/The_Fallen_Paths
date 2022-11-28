
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static EnemyInfo;

//il faut qu'il recoive la dernière position ou il a vu le player
public class DetectionToSpotEnemy : MonoBehaviour
{
    private EnemyDetectionManager _detectionManager;
    private CoroutineProcessor _coroutineProcessor;
    private AEnemyMovement _movementManager;

    protected bool _isDisable = true;
    protected CoroutineProcessor _processor;
    protected Coroutine _coroutine;
    protected Transform _interact;

    
    private void Start()
    {
        _detectionManager = GetComponent<EnemyDetectionManager>();
        _coroutineProcessor = GetComponent<CoroutineProcessor>();
        _movementManager = GetComponent<AEnemyMovement>();

        _isDisable = true;
        _processor = GetComponent<CoroutineProcessor>();
    }
    
    public void Update()
    {
        //Debug.Log("_processor.crRunning : " + _processor.crRunning);

        if (!_isDisable)
        {
            if (!_processor.crRunning && _coroutine == null) {
                if (_interact == null)
                    return;
                // added this
                if (RangeOf(_interact.position.x, transform.position.x, DistanceToInteract.x) && 
                    RangeOf(_interact.position.y, transform.position.y, 0.25f)) {
                    SetState(EnemyEventState.SeenRandomSpoted);
                    _coroutine = StartCoroutine(Trigger(_interact));
                    //! added this
                }
            }
        } else {
            if (_coroutine != null) {
                _coroutine = null;
            }
        }
    }
    
    public IEnumerator Trigger(Transform obj = null)
    {
        if (_processor != null) {
            yield return StartCoroutine(_processor.Interact(obj));
        }
    }

    public void SetInteractionObj(Transform interact)
    {
        if (interact != null) {
            _interact = interact;
        }
    }

    public void SetState(EnemyEventState state)
    {
        if (_processor != null) {
            _processor.enemyState = state;
        }
    }

    public void SetDisabling(bool isDisable)
    {
        if (isDisable) {
            if (_processor.crRunning) {
                StopCoroutine(Trigger());
                StopCoroutine(_processor.Interact());
            }
            _isDisable = true;
            _processor.crRunning = false;
        } else {
            _coroutine = null;
            _isDisable = false;
        }
    }
}
