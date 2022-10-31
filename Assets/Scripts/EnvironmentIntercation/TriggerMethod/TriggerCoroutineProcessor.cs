using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ou alors faire un trigger qui lance un raycats juste pour l'enemmy

public class TriggerCoroutineProcessor : MonoBehaviour
{
    protected bool _isDisable = true;
    protected CoroutineProcessor _processor;
    protected Coroutine _coroutine;

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
                _coroutine = StartCoroutine(Trigger());
            }
        } else {
            if (_coroutine != null) {
                _coroutine = null;
            }
        }
    }
    
    public IEnumerator Trigger()
    {
        Debug.Log("Trigger");
        if (_processor != null) {
            yield return StartCoroutine(_processor.Interact());
        }
    }

    public void SetState(EnemyEventState state)
    {
        _processor.enemyState = state;
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
            _isDisable = false;
        }
    }
}