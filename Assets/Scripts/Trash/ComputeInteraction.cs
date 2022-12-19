using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputeInteraction : MonoBehaviour
{
    private TriggerProcessor _interactor;
    private PowerMenuManager _powerUiManager;
    private bool _blockInput = false;

    private void Start()
    {
        _interactor = null;
        _powerUiManager = (PowerMenuManager)FindObjectOfType<PowerMenuManager>();
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
            
            //pas ouf en boucle comme ça
            var checkPoints = FindObjectsOfType<CheckpointInteraction>();
            
            if (checkPoints.Length == 0) {
                return;
            }
            if (_interactor.gameObject.GetComponent<CheckpointInteraction>() != null) {
                if (Input.GetKeyDown(KeyCode.M)) 
                {
                    _powerUiManager.AblePowerMenu();
                }
            }
        }
    }
}
