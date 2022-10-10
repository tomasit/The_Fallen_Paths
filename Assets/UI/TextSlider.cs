using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextSlider : MonoBehaviour
{
    public TextMeshProUGUI textMeshProUGUI;


    public void SetNumberText(float value)
    {
        textMeshProUGUI.text = value.ToString();
    }
}

