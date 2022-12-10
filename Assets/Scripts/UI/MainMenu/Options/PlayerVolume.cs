using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerVolume : MonoBehaviour
{
    public Slider plVolume;
    public TextMeshProUGUI textValue;
    Parameters param;

    private void Start()
    {
        param = SaveManager.DataInstance.GetParameters();
        plVolume.value = param._subVolume.GetValueOrDefault(SoundData.SoundEffectType.ENVIRONMENT);
        textValue.text = Mathf.Round(plVolume.value * 100).ToString();

    }

    public void SetPlaVolume()
    {
        param._subVolume[SoundData.SoundEffectType.PLAYER] = plVolume.value;
        textValue.text = Mathf.Round(plVolume.value * 100).ToString();
        SaveManager.DataInstance.SaveParameters();
    }
}
