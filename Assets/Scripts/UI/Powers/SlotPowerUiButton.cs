using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotPowerUiButton : MonoBehaviour
{
    [HideInInspector] public int powerIndex;
    [HideInInspector] public int slotIndex;
    [HideInInspector] public KeyCode key;
    private PowerGUIManager _powerGUiManager;
    private PowerManager _powerManager;

    private void Start()
    {
        _powerGUiManager = (PowerGUIManager)FindObjectOfType(typeof(PowerGUIManager));
        _powerManager = (PowerManager)FindObjectOfType(typeof(PowerManager));
    }

    public void TriggerAssignPower()
    {
        _powerManager.AssignKey(powerIndex, key);
        _powerGUiManager.AddPower(powerIndex, slotIndex);
    }
}