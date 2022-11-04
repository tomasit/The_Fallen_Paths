using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PowerManager : MonoBehaviour
{
    [System.Serializable]
    private class PowerData
    {
        public APower power;
        [System.NonSerialized] public float cooldown = 0;
        public float cooldownDuration;
        [System.NonSerialized] public float duration = 0;
        public float maxDuration = -1;
    }
    [SerializeField]
    private List<PowerData> _powers;
    public int _currentPowerIndex = -1;

    public bool canUseAnyPower = true;

    // Start is called before the first frame update
    void Start()
    {

    }

    void ChoosePower(int powerIndex)
    {
        if (powerIndex < 0 || powerIndex > _powers.Count - 1)
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
            if (currentPowerData.cooldown <= 0)
            {
                currentPowerData.cooldown = 0;
                _currentPowerIndex = powerIndex;
                currentPowerData.power.Use();
                currentPowerData.cooldown = currentPowerData.cooldownDuration;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // NOTE: best debug ever
        for (int i = 0; i < _powers.Count; ++i)
        {
            if (Input.GetKeyDown(KeyCode.A + i))
            {
                Debug.Log("key: " + i);
                ChoosePower(i);
            }
        }
        Debug.Log("first:" + _currentPowerIndex);
        if (_currentPowerIndex >= 0)
        {
            var currentPowerData = _powers[_currentPowerIndex];
            var currentPower = currentPowerData.power;
            Debug.Log("mood: " + currentPower.GetType().Name);
            if (currentPower.firingPower == false)
            {
                if (currentPower is ARangedPower)
                {
                    if ((currentPower as ARangedPower).activated == false)
                        _currentPowerIndex = -1;
                }
                else
                {
                    currentPowerData.duration = 0;
                    _currentPowerIndex = -1;
                }
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
                power.cooldown -= Time.deltaTime;
        Debug.Log("at the end:" + _currentPowerIndex);
    }
}
