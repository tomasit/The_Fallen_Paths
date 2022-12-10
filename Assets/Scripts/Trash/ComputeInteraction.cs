using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputeInteraction : MonoBehaviour
{
    private TriggerProcessor _interactor;
    private bool _blockInput = false;

    private void Start()
    {
        _interactor = null;
    }

    public void BlockInput(bool block)
    {
        _blockInput = block;
    }

    private void OnTriggerEnter2D(Collider2D hit)
    {
        if (hit.GetComponent<TriggerProcessor>() != null)
            _interactor = hit.GetComponent<TriggerProcessor>();
    }

    private void OnTriggerExit2D(Collider2D hit)
    {
        if (hit.GetComponent<TriggerProcessor>() != null)
            _interactor = null;
    }

    private void OnCollisionEnter2D(Collision2D hit)
    {
        if (hit.collider.GetComponent<TriggerProcessor>() != null)
            _interactor = hit.collider.GetComponent<TriggerProcessor>();
    }

    private void OnCollisionExit2D(Collision2D hit)
    {
        if (hit.collider.GetComponent<TriggerProcessor>() != null)
            _interactor = null;
    }

    private void Update()
    {
        if (_interactor != null && !_blockInput)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                _interactor.Trigger();
            }
        }
    }
}
