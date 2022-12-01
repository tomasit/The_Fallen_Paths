using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCoroutineProcessor : MonoBehaviour
{
    protected bool _isDisable = true;
    protected CoroutineProcessor _processor;
    protected Coroutine _coroutine;
    protected Transform _interact;

    public void Start()
    {
        _isDisable = true;
        _processor = GetComponent<CoroutineProcessor>();
    }

    public void Update()
    {
        if (!_isDisable)
        {
            if (!_processor.crRunning && _coroutine == null) {
                _coroutine = StartCoroutine(Trigger(_interact));
            }
        } else {
            //il est sensé passer ici zebi
            if (_coroutine != null) {
                Debug.Log("_isDisable == true");
                Debug.Log("coroutine = null");
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

    //use in extreme case
    public void SetCoroutine(Coroutine coroutine)
    {
        _coroutine = coroutine;
    }

    public void SetInteractionObj(Transform interact)
    {
        if (interact != null) {
            _interact = interact;
        }
    }

    public void SetState(EnemyEventState state)
    {
        _processor.enemyState = state;
    }

    public void SetDisabling(bool isDisable)
    {
        if (isDisable) {
            //Debug.Log("---_isDisable : " + _isDisable);
            if (_processor.crRunning) {
                //Debug.Log("---Stop Coroutines");
                StopCoroutine(Trigger());
                StopCoroutine(_processor.Interact());
            }
            //Debug.Log("---Set _isDisable = true & crRunning = false");
            _isDisable = true;
            _processor.crRunning = false;
        } else {
            _isDisable = false;
        }
    }
}