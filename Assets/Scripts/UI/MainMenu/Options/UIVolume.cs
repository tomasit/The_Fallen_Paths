using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIVolume : MonoBehaviour
{
    public Slider menVolume;
    public TextMeshProUGUI textValue;
    Parameters param;

    private void Start()
    {
        param = SaveManager.DataInstance.GetParameters();
        menVolume.value = param._subVolume.GetValueOrDefault(SoundData.SoundEffectType.UI);
        textValue.text = Mathf.Round(menVolume.value * 100).ToString();

    }

    public void SetUIVolume()
    {
        param._subVolume[SoundData.SoundEffectType.UI] =menVolume.value;
        textValue.text = Mathf.Round(menVolume.value * 100).ToString();
        SaveManager.DataInstance.SaveParameters();
    }
}
