using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeHandler : MonoBehaviour
{
    public Slider volumeSlider;
    public void SetVolume()
    {
        AudioListener.volume = volumeSlider.value;
    }
}
