using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionManager : MonoBehaviour
{
    public TextMeshProUGUI textMeshProUGUI;
    public Slider volumeSlider;
    public TMP_Dropdown dropdown;

    private bool fs = false;
    private int width = 1920;
    private int height = 1080;
    public void SetVolume()
    {
        AudioListener.volume = volumeSlider.value;
    }

    public void SetNumberText(float value)
    {
        textMeshProUGUI.text = value.ToString();
    }

    public void OnChange()
    {
        if (dropdown.value == 0)
        {
            fs = false;
            Screen.SetResolution(width, height, fs);
        } else if (dropdown.value == 1)
        {
            fs = true;
            Screen.SetResolution(width, height, fs);
        }
    }
}
