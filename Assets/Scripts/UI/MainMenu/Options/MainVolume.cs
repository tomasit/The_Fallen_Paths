using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class MainVolume : MonoBehaviour
{
    public Slider volumeGlobal;
    public TextMeshProUGUI textValue;
    Parameters param;

    void Start()
    {
        param = SaveManager.DataInstance.GetParameters();
        volumeGlobal.value = param._globalVolume;
        textValue.text = Mathf.Round(volumeGlobal.value * 100).ToString();

    }

    public void SetGlobalVolume()
    {
        param._globalVolume = volumeGlobal.value;
        textValue.text = Mathf.Round(volumeGlobal.value * 100).ToString();
        SaveManager.DataInstance.SaveParameters();
    }
}
