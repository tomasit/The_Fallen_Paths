using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestComputeInteraction : MonoBehaviour
{
    private CollisionDetection _interactor;
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
        if (hit.GetComponent<CollisionDetection>() != null)
            _interactor = hit.GetComponent<CollisionDetection>();
    }

    private void OnTriggerExit2D(Collider2D hit)
    {
        if (hit.GetComponent<CollisionDetection>() != null)
            _interactor = null;
    }

    private void OnCollisionEnter2D(Collision2D hit)
    {
        if (hit.collider.GetComponent<CollisionDetection>() != null)
            _interactor = hit.collider.GetComponent<CollisionDetection>();
    }

    private void OnCollisionExit2D(Collision2D hit)
    {
        if (hit.collider.GetComponent<CollisionDetection>() != null)
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
