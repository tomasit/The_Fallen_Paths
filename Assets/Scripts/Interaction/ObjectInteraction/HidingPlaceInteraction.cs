using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HidingPlaceInteraction : AInteractable
{
    // here not putting an AInteraction because this interaction is very specific and need
    // the hide interaction
    public UnityEvent _hideEvent;
    public UnityEvent<bool> _lightEvent;
    public bool _isLitPlace = false;
    private bool _containPlayer = false;
    private bool _isAlight = false;

    public void NotifyByLight(bool isAlight)
    {
        if (!_isLitPlace)
            return;
    
        _isAlight = isAlight;
        Debug.Log("Alight = " + isAlight);

        if (_containPlayer)
        {
           _lightEvent.Invoke(_isAlight);
        }
    }

    public override void Interact()
    {
        _containPlayer = !_containPlayer;
        _hideEvent.Invoke();

        if (_isLitPlace)
        {
            Debug.Log("Interaction : Am i alight ? " + _isAlight);
            _lightEvent.Invoke(_isAlight);
        }
    }

    public override void Load()
    {
    }

    public override void Save()
    {
    }
}
