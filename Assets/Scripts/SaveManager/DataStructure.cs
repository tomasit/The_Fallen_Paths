using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerInfoSave
{
    public string _playerName = "";
}

[System.Serializable]
public class Parameters
{
    public float _globalVolume = 1.0f;
    public Dictionary<SoundData.SoundEffectType, float> _subVolume;
}