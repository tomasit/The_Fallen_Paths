using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnvVolume : MonoBehaviour
{
    public Slider envVolume;
    public TextMeshProUGUI textValue;
    Parameters param;

    private void Start()
    {
        param = SaveManager.DataInstance.GetParameters();
        envVolume.value = param._subVolume.GetValueOrDefault(SoundData.SoundEffectType.ENVIRONMENT);
        textValue.text = Mathf.Round(envVolume.value * 100).ToString();
    }

    public void SetEnvVolume() {
        param._subVolume[SoundData.SoundEffectType.ENVIRONMENT] = envVolume.value;
        textValue.text = Mathf.Round(envVolume.value * 100).ToString();
        SaveManager.DataInstance.SaveParameters();
    }
}
