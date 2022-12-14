using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Events;
using System;

public class PowerManager : MonoBehaviour
{
    [System.Serializable]
    public class PowerData
    {
        public APower power;
        [System.NonSerialized] public float cooldown = 0;
        public float cooldownDuration;
        [System.NonSerialized] public float duration = 0;
        public float maxDuration = -1;
        public bool unlocked = false;
        public bool cancelable = false;
        public bool powerManageItsDuration = true;
        public KeyCode key;
    }
    [SerializeField]
    public List<PowerData> _powers;
    private int _currentPowerIndex = -1;
    public bool canUseAnyPower = true;
    private UnityEvent<System.Type> _unlockPowerEvent;

    /*private Dictionary<int, KeyCode> Load()
    {
        return SaveManager.DataInstance.GetPowers()._powersUnlocked;
    }*/
    private List<int> Load()
    {
        return SaveManager.DataInstance.GetPowers()._powersIndex;
    }

    private void Save()
    {
        SaveManager.DataInstance.SavePowers();
    }

    private void Start()
    {
        LoadPowersFromSave();
    }

    public void LoadPowersFromSave()
    {
        //Dictionary<int, KeyCode> listPowersSaved = Load();
        List<int> listPowersSaved = Load();

        for (int idx = 0; idx < listPowersSaved.Count; ++idx)
        {
            /*var keyValue = listPowersSaved.ElementAt(idx);
            if (keyValue.key != -1) {
                if (_powers.Count > keyValue.key) {
                    _powers[keyValue.key].unlocked = true;
                    _powers[keyValue.key].key = keyValue.value;         
                }
            }*/
            if (listPowersSaved[idx] != -1) {
                if (_powers.Count > listPowersSaved[idx]) {
                    _powers[listPowersSaved[idx]].unlocked = true;
                }
            }
        }
    }

    public void UnlockPower(int idx)
    {
        _powers[idx].unlocked = true;

        List<int> indexPowerSaved = Load();
        indexPowerSaved[idx] = idx;
        Save();
    }

    public void AssignKey(int powerIdx, KeyCode newKey) {
        if (powerIdx < _powers.Count) {
            _powers[powerIdx].key = newKey;
        } else {
            Debug.Log("Could not add key");
        }
    }

    public List<PowerData> GetPowers()
    {
        return _powers;
    }

    public UnityEvent<System.Type> Observe()
    {
        return _unlockPowerEvent;    
    }

    public void UnlockPower(System.Type powerType)
    {
        _powers[FindPowerIndex(powerType)].unlocked = true;
        _unlockPowerEvent.Invoke(powerType);
    }

    public int FindPowerIndex(System.Type powerType)
    {
        return _powers.FindIndex(x => x.power.GetType() == powerType);
    }

    public float GetPowerMaxDurationFromStackTrace(int frame)
    {
        var callerPowerType = new System.Diagnostics.StackTrace().GetFrame(frame).GetMethod().ReflectedType;
        int powerIndex = FindPowerIndex(callerPowerType);
        return _powers[powerIndex].maxDuration;
    }

    public void ActivatePowerCooldownFromStackTrace(int frame = 1)
    {
        var callerPowerType = new System.Diagnostics.StackTrace().GetFrame(frame).GetMethod().ReflectedType;
        Debug.Log("Activate cooldown for " + callerPowerType.Name);
        int powerIndex = FindPowerIndex(callerPowerType);
        _powers[powerIndex].cooldown = _powers[powerIndex].cooldownDuration;
    }

    public void ResetEverything()
    {
        CancelCurrentPowerState();
        foreach (var power in _powers)
        {
            power.cooldown = 0;
            power.duration = 0;
        }
    }

    private void CancelCurrentPowerState(bool canCancelFire = true)
    {
        if (_currentPowerIndex == -1)
            return;

        var currentPower = _powers[_currentPowerIndex].power;
        if (currentPower.firingPower == false && currentPower is ARangedPower)
        {
            (currentPower as ARangedPower).CancelRange();
        }
        else if (canCancelFire && currentPower.firingPower == true)
            currentPower.Cancel();
        _currentPowerIndex = -1;
    }

    public void ChoosePower(System.Type powerType)
    {
        int powerIndex = FindPowerIndex(powerType);

        if (powerIndex < 0)
            return;
        if (powerIndex == _currentPowerIndex)
        {
            CancelCurrentPowerState(_powers[powerIndex].cancelable);
        }
        else if (canUseAnyPower && _currentPowerIndex != -1
            && (_powers[_currentPowerIndex].power is ARangedPower)
            && (_powers[_currentPowerIndex].power as ARangedPower).activated == true
            && (_powers[powerIndex].cooldown <= 0)
            && (_powers[powerIndex].unlocked))
        {
            (_powers[_currentPowerIndex].power as ARangedPower).CancelRange();
            _powers[powerIndex].power.Use();
            _currentPowerIndex = powerIndex;
        }
        else if (canUseAnyPower &&
        (_currentPowerIndex == -1
        || (_powers[_currentPowerIndex].power.firingPower == true
        && _powers[_currentPowerIndex].powerManageItsDuration == true)))
        {
            var currentPowerData = _powers[powerIndex];
            if (currentPowerData.cooldown <= 0 && currentPowerData.unlocked)
            {
                _currentPowerIndex = powerIndex;
                currentPowerData.power.Use();
            }
        }
    }

    void Update()
    {
        foreach (var data in _powers)
            if (Input.GetKeyDown(data.key))
            {
                ChoosePower(data.power.GetType());
            }

        if (_currentPowerIndex >= 0)
        {
            var currentPowerData = _powers[_currentPowerIndex];
            var currentPower = currentPowerData.power;
            if (currentPower.firingPower == false)
            {
                if (currentPower is ARangedPower)
                {
                    if ((currentPower as ARangedPower).activated == false)
                        _currentPowerIndex = -1;
                }
                else
                {
                    _currentPowerIndex = -1;
                }
                currentPowerData.duration = 0;
            }
        }
        foreach (var power in _powers)
        {
            if (power.cooldown > 0)
            {
                power.cooldown = Mathf.Max(power.cooldown - Time.deltaTime, 0);
                //Debug.Log(power.power.GetType().Name + " cooldown: " + power.cooldown);
            }
            if (power.powerManageItsDuration == false && power.maxDuration != -1 && power.power.firingPower == true)
            {
                power.duration += Time.deltaTime;
                if (power.duration >= power.maxDuration)
                {
                    power.power.Cancel();
                    power.duration = 0;
                    _currentPowerIndex = -1;
                }
            }
        }
    }
}