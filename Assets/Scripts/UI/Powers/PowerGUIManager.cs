using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

[System.Serializable]
public struct PowerGIUSlot
{
    public bool exist;
    public Image PowerIcon;
    public GUI_Cooldown CoolDown;
    public int powerIndex;
    public string KeyValue;
}

public class PowerGUIManager : MonoBehaviour
{
    [Header("Basics")]
    [SerializeField] private bool _isDisplay;
    [SerializeField] private GameObject _powerGui;

    [Header("Slots")]
    [SerializeField] public PowerGIUSlot [] _powersGui;
    [SerializeField] private GameObject _powerGuiSlotPrefab;
    public uint maxSlots = 3;

    private PowerManager _powerManager;
    private PowerMenuManager _powerUiManger;

    private void Start()
    {
        _powerManager = (PowerManager)FindObjectOfType<PowerManager>();
        _powerUiManger = (PowerMenuManager)FindObjectOfType<PowerMenuManager>();

        _powersGui = new PowerGIUSlot[maxSlots];
        for (int idx = 0; idx < _powersGui.Length; ++idx) {
            _powersGui[idx].powerIndex = -1;
        }

        //replace by load & save
        LoadGuiSlots();
    }

    private List<int> Load()
    {
        return SaveManager.DataInstance.GetGuiPowers()._powersGuiIndex;
    }

    private void Save()
    {
        SaveManager.DataInstance.SaveGuiPowers();
    }

    private void Update()
    {
        if (!_isDisplay) {
            _powerGui.SetActive(false);
        } else {
            Display();
        }
    }

    private void Display()
    {
        int powerUnlocked = 0;
        foreach (var powerGui in _powersGui)
        {
            if (powerGui.exist) {
                powerUnlocked += 1;
            }
        }
        if (powerUnlocked == 0) {
            _powerGui.SetActive(false);
        } else {
            //LoadGuiSlots();
            _powerGui.SetActive(true);
        }
    }

    public void LoadGuiSlots()
    {
        List<int> powersAssigned = Load();

        /*var idexesPowerToAdd = new int [] {-1, -1, -1};
        int idxPowerToAdd = 0;
        foreach (var powerGui in _powersGui)
        {
            if (powerGui.exist) {
                idexesPowerToAdd[idxPowerToAdd] = powerGui.powerIndex;
                ++idxPowerToAdd;
            }
        }*/
        foreach (var pow in powersAssigned) {
            Debug.Log("Set Save power : " + pow);
        }
        Debug.Log("_______________");
        SetPowers(powersAssigned.ToArray());
        SetUpGuiSlots();
    }

    public void AddPower(int indexPower, int slotNb)
    {
        Debug.Log("Add power");
        List<int> powersAssigned = Load();
        powersAssigned[slotNb] = indexPower;
        foreach (var pow in powersAssigned) {
            Debug.Log("Save : " + pow);
        }
        Debug.Log("__________");
        Save();

        var idexesPowerToAdd = new int [maxSlots];

        int idx = 0;

        foreach(var power in _powersGui) {
            if (idx >= maxSlots) {
                break;
            }
            var powerIdx = power.powerIndex;
            if (idx == slotNb) {
                powerIdx = indexPower;
            }
            idexesPowerToAdd[idx] = powerIdx;
            ++idx;
        }
        SetPowers(idexesPowerToAdd);
        DestroyGuiSlots();
        SetUpGuiSlots();
    }

    public void SetPowers(int [] powersToAssign)
    {
        int idxArrayPowerToAssign = 0;
        int idxPowerGui = 0;
        
        // clear array
        if (_powersGui.Length != 0) {
            Array.Clear(_powersGui, 0, _powersGui.Length);
        }
        for (int i = 0; i < _powersGui.Length; ++i) {
            _powersGui[i].powerIndex = -1;
        }
        // !clear array

        for (int idx = 0; idx < _powerManager._powers.Count; ++idx)
        {
            if (idxArrayPowerToAssign >= powersToAssign.Length) {
                return;
            }
            if (idxPowerGui >= maxSlots) {
                return;
            }
            var power = _powerManager._powers[idx];
            var idxPowerToAssign = powersToAssign[idxArrayPowerToAssign];

            //if ((Array.IndexOf(powersToAssign, idx) != -1) && power.unlocked) {
            if ((powersToAssign[idxArrayPowerToAssign] != -1) /*&& power.unlocked*/) {
                _powersGui[idxPowerGui].exist = true;
                _powersGui[idxPowerGui].powerIndex = idxPowerToAssign;
                //mettre la key qu'on nous donne sur _powerManager
                _powersGui[idxPowerGui].KeyValue = power.key.ToString();

                if (_powerUiManger._uiPowers[idxPowerToAssign].ImagePowerUnLocked == null) {
                    Debug.Log("idxPowerToAssign = " + idxPowerToAssign);
                }
                _powersGui[idxPowerGui].PowerIcon = _powerUiManger._uiPowers[idxPowerToAssign].ImagePowerUnLocked.GetComponent<Image>();

                ++idxPowerGui;
                ++idxArrayPowerToAssign;
            }
        }
    }

    public void DestroyGuiSlots()
    {
        var coolDowns = FindObjectsOfType<GUI_Cooldown>();

        foreach (var coolDown in coolDowns) 
        {
            UnityEngine.Object.Destroy(coolDown.gameObject.transform.parent.transform.gameObject);
        }
    }

    public void SetUpGuiSlots()
    {
        for (int idx = 0; idx < _powersGui.Length; ++idx) {
            var powerGui = _powersGui[idx];

            if (!powerGui.exist) {
                return;
            }

            GameObject newSlot = UnityEngine.Object.Instantiate(
                _powerGuiSlotPrefab,
                transform.position,
                transform.rotation,
                _powerGui.transform);

            newSlot.transform.localScale = new Vector3(1f, 1f, 1f);
            
            newSlot.transform.GetChild(0).GetComponent<Image>().sprite = powerGui.PowerIcon.sprite;
            newSlot.transform.GetChild(0).Find("KeyValueText").GetComponent<TMP_Text>().text = "[" + powerGui.KeyValue + "]";

            _powersGui[idx].CoolDown = newSlot.transform.GetChild(0).GetComponent<GUI_Cooldown>();
            _powersGui[idx].CoolDown.PowerIndex = powerGui.powerIndex;
        }
    }
}