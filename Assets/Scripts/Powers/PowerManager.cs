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
        public KeyCode key;
    }
    [SerializeField]
    public List<PowerData> _powers;
    private int _currentPowerIndex = -1;

    public bool canUseAnyPower = true;

    // Start is called before the first frame update
    void Start()
    {

    }

    int FindPowerIndex(string powerName)
    {
        return _powers.FindIndex(x => x.power.GetType() == System.Type.GetType(powerName));
    }

    public void ChoosePower(string powerName)
    {
        int powerIndex = FindPowerIndex(powerName);

        if (powerIndex < 0)
            return;
        if (powerIndex == _currentPowerIndex)
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
        else if (canUseAnyPower && _currentPowerIndex == -1 && powerIndex >= 0)
        {
            var currentPowerData = _powers[powerIndex];
            if (currentPowerData.cooldown <= 0 && currentPowerData.unlocked)
            {
                currentPowerData.cooldown = 0;
                _currentPowerIndex = powerIndex;
                currentPowerData.power.Use();
                currentPowerData.cooldown = currentPowerData.cooldownDuration;
            }
        }
    }

    void Update()
    {
        foreach (var data in _powers)
            if (Input.GetKeyDown(data.key))
            {
                ChoosePower(data.power.GetType().Name);
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
            else if (currentPowerData.maxDuration != -1)
            {
                currentPowerData.duration += Time.deltaTime;
                if (currentPowerData.duration >= currentPowerData.maxDuration)
                {
                    currentPower.Cancel();
                    currentPowerData.duration = 0;
                    _currentPowerIndex = -1;
                }
            }
        }
        foreach (var power in _powers)
            if (power.cooldown > 0)
                power.cooldown = Mathf.Max(power.cooldown - Time.deltaTime, 0);
    }
}
