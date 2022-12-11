using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MusicVolume : MonoBehaviour
{
    public Slider mscVoume;
    public TextMeshProUGUI textValue;
    Parameters param;

    private void Start()
    {
        param = SaveManager.DataInstance.GetParameters();
        mscVoume.value = param._subVolume.GetValueOrDefault(SoundData.SoundEffectType.MUSIC);
        textValue.text = Mathf.Round(mscVoume.value * 100).ToString();

    }

    public void SetMusVolume()
    {
        param._subVolume[SoundData.SoundEffectType.MUSIC] = mscVoume.value;
        textValue.text = Mathf.Round(mscVoume.value * 100).ToString();
        SaveManager.DataInstance.SaveParameters();
    }
}
