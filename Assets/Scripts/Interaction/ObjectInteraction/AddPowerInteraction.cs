using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddPowerInteraction : AInteractable
{
    [SerializeField] private int _idxPowerToAdd;
    private PowerManager _powerManager;
    private PowerMenuManager _powerUiManager;

    private void Start()
    {
        _powerManager = (PowerManager)FindObjectOfType<PowerManager>();
        _powerUiManager = (PowerMenuManager)FindObjectOfType<PowerMenuManager>();
    }

    public override void Save()
    {
    }

    public override void Load()
    {
    }

    public override void Interact()
    {
        for (int idx = 0; idx < _powerManager._powers.Count; ++idx) 
        {
            if (idx == _idxPowerToAdd) {
                _powerManager._powers[idx].unlocked = true;
            }
        }
        _powerUiManager.AblePowerMenu();
    }
}