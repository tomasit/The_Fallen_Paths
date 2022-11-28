using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HidingPlaceInteraction : AInteractable
{
    public UnityEvent<bool, bool> _hideEvent;
    public UnityEvent<bool> _lightEvent;
    public bool _isLitPlace = false;
    private bool _containPlayer = false;
    private bool _isAlight = false;

    public void NotifyByLight(bool isAlight)
    {
        if (!_isLitPlace)
            return;
    
        _isAlight = isAlight;
        
        if (_containPlayer)
        {
           _lightEvent.Invoke(_isAlight);
        }
    }

    public override void Interact()
    {
        _containPlayer = !_containPlayer;
        _hideEvent.Invoke(_containPlayer, _isLitPlace);

        if (_isLitPlace && _containPlayer)
        {
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
