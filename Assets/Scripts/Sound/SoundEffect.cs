using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundEffect : MonoBehaviour
{
    private AudioSource _source;
    [SerializeField] private SoundData.SoundEffectType _soundType;

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
    }

    public void PlaySound(SoundData.SoundEffectName effectType)
    {
        if (_source.isPlaying)
            _source.Stop();
        float volume = (SaveManager.DataInstance.GetParameters()._globalVolume > SaveManager.DataInstance.GetParameters()._subVolume[_soundType] ?
            SaveManager.DataInstance.GetParameters()._subVolume[_soundType] : SaveManager.DataInstance.GetParameters()._globalVolume);
        _source.PlayOneShot(SoundData.SoundEffect[effectType], volume);
    }
}