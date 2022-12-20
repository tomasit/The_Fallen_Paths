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
    public int _width = 1920;
    public int _height = 1080;
    public bool _fullscreen = true;
    public Dictionary<SoundData.SoundEffectType, float> _subVolume;
}

[System.Serializable]
public class Powers
{
    public List<int> _powersIndex;
    public Dictionary<int, KeyCode> _powersUnlocked;
}

[System.Serializable]
public class PowersGui
{
    public List<int> _powersGuiIndex;
}