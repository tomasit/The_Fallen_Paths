using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionManager : MonoBehaviour
{
    public Dropdown res_dropdown;
    public Dropdown win_dropdown;
    public TextMeshProUGUI textMeshProUGUI;
    public Slider volumeGlobal;
    public Slider volumeEnv;
    private bool fs = true;
    private int width = 1920;
    private int height = 1080;
    Parameters param;

    private void Start()
    {
        param = SaveManager.DataInstance.GetParameters();
    }
    public void SetGlobalVolume()
    {
        param._globalVolume = volumeGlobal.value;
        SaveManager.DataInstance.SaveParameters();
    }

    public void SetEnvVolume()
    {
        param._subVolume.Add(SoundData.SoundEffectType.ENVIRONMENT, volumeEnv.value);
        SaveManager.DataInstance.SaveParameters();
    }

    public void SetNumberText(float value)
    {
        textMeshProUGUI.text = value.ToString();
    }

    public void OnScreenChange()
    {
        if (win_dropdown.value == 0)
        {
            fs = true;
            Screen.SetResolution(width, height, fs);
        }
        else if (win_dropdown.value == 1)
        {
            fs = false;
            Screen.SetResolution(width, height, fs);
        }
        param._fullscreen = fs;
        SaveManager.DataInstance.SaveParameters();
    }

    public void OnResChange()
    {
        switch (res_dropdown.value)
        {
            case 0:
                {
                    width = 1920;
                    height = 1080;
                    Screen.SetResolution(width, height, fs);
                    break;
                }
            case 1:
                    width = 1280;
                    height = 720;
                    Screen.SetResolution(width, height, fs);
                    break;
                {
                }
            case 2:
                {
                    width = 2560;
                    height = 1440;
                    Screen.SetResolution(width, height, fs);
                    break;
                }
        }
        param._width = width;
        param._height = height;
        SaveManager.DataInstance.SaveParameters();
    }
}
