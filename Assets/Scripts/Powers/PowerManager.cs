using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

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

        public bool powerManageItsDuration = true;
        public KeyCode key;
    }
    [SerializeField]
    public List<PowerData> _powers;
    private int _currentPowerIndex = -1;

    public bool canUseAnyPower = true;


    int FindPowerIndex(System.Type powerType)
    {
        return _powers.FindIndex(x => x.power.GetType() == powerType);
    }

    public float GetPowerMaxDurationFromStackTrace(int frame)
    {
        var callerPowerType = new System.Diagnostics.StackTrace().GetFrame(frame).GetMethod().ReflectedType;
        int powerIndex = FindPowerIndex(callerPowerType);
        return _powers[powerIndex].maxDuration;
    }

    public void ActivatePowerCooldownFromStackTrace(int frame)
    {
        var callerPowerType = new System.Diagnostics.StackTrace().GetFrame(frame).GetMethod().ReflectedType;
        Debug.Log("Activate cooldown for " + callerPowerType.Name);
        int powerIndex = FindPowerIndex(callerPowerType);
        _powers[powerIndex].cooldown = _powers[powerIndex].cooldownDuration;
    }

    public void ResetEverything()
    {
        if (_currentPowerIndex != -1)
            CancelCurrentPowerState();
        foreach (var power in _powers)
        {
            power.cooldown = 0;
            power.duration = 0;
        }
    }

    private void CancelCurrentPowerState()
    {
        var currentPower = _powers[_currentPowerIndex].power;
        if (currentPower.firingPower == false && currentPower is ARangedPower)
        {
            (currentPower as ARangedPower).CancelRange();
        }
        else if (currentPower.firingPower == true)
            currentPower.Cancel();
        _currentPowerIndex = -1;
    }

    public void ChoosePower(System.Type powerType)
    {
        int powerIndex = FindPowerIndex(powerType);

        if (powerIndex < 0)
            return;
        if (powerIndex == _currentPowerIndex)
            CancelCurrentPowerState();
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
        (_currentPowerIndex == -1 || (_powers[_currentPowerIndex].power.firingPower == true && _powers[_currentPowerIndex].powerManageItsDuration == true)))
        {
            var currentPowerData = _powers[powerIndex];
            if (currentPowerData.cooldown <= 0 && currentPowerData.unlocked)
            {
                currentPowerData.cooldown = 0;
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
                Debug.Log(power.power.GetType().Name + " cooldown: " + power.cooldown);
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
