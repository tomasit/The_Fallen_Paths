using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestComputeInteraction : MonoBehaviour
{
    private CollisionDetection _interactor;

    private void Start()
    {
        _interactor = null;
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

    private void Update()
    {
        if (_interactor != null)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                _interactor.Trigger();
            }
        }
    }
}
