using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ObjectData = System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, object>>;

public class SaveManager : MonoBehaviour
{
    private static SaveManager _dataInstance = null;

    public static SaveManager DataInstance {
        get {
            if (_dataInstance == null) {

                _dataInstance = FindObjectOfType<SaveManager>();
                if (_dataInstance == null) {
                    _dataInstance = new GameObject().AddComponent<SaveManager>();
                }
            }
            return _dataInstance;
        }
    }

    private List<ObjectData> _levelData;
    private PlayerInfoSave _playerInfo;
    private Parameters _parameters;
    private Powers _powers;
    private PowersGui _guiPowers;
    private int _currentLevel = 0;
    [HideInInspector] public uint _nbKey = 0;

    private void Awake() {
        Debug.Log("Singleton awake");
        if (_dataInstance != null) {
            Debug.Log("Singleton always present on the scene, destroy this one");
            Destroy(this);
        } else {
            DontDestroyOnLoad(this);
        }
        _levelData = new List<ObjectData>();
        _playerInfo = new PlayerInfoSave();
        _parameters = new Parameters();
        _parameters._subVolume = new Dictionary<SoundData.SoundEffectType, float>();
        _powers = new Powers();
        _powers._powersIndex = new List<int>();
        //_powers._powersUnlocked = new Dictionary<int, KeyCode>();
        _guiPowers = new PowersGui();
        _guiPowers._powersGuiIndex = new List<int>();
        Load();
    }

    public Parameters GetParameters()
    {
        return _parameters;
    }

    public PlayerInfoSave GetPlayerInfo()
    {
        return _playerInfo;
    }

    public bool IsReferenced(string objectId, string valueName)
    {
        if (!_levelData[_currentLevel].ContainsKey(objectId))
        {
            return false;
        }
        return _levelData[_currentLevel][objectId].ContainsKey(valueName);
    }

    public object GetValue(string objectId, string valueName)
    {
        if (!IsReferenced(objectId, valueName))
        {
            Debug.Log("You are trying to get a non-save value");
            return null;
        }
        return _levelData[_currentLevel][objectId][valueName];
    }

    public void ReferenceValue(string objectId, string nameOfValue, object valueToReference)
    {
        if (!_levelData[_currentLevel].ContainsKey(objectId))
        {
            _levelData[_currentLevel].Add(objectId, new Dictionary<string, object>());
        }
        if (!_levelData[_currentLevel][objectId].ContainsKey(nameOfValue))
        {
            _levelData[_currentLevel][objectId].Add(nameOfValue, valueToReference);
        }
        else
        {
            _levelData[_currentLevel][objectId][nameOfValue] = valueToReference;
        }
    }

    public void Load()
    {
        // Load parameters
        if (SerializationManager.Exist("Parameters"))
        {
            _parameters = (Parameters)SerializationManager.Load("Parameters");
        }
        else
        {
            _parameters._subVolume.Add(SoundData.SoundEffectType.MUSIC, 1.0f);
            _parameters._subVolume.Add(SoundData.SoundEffectType.PLAYER, 1.0f);
            _parameters._subVolume.Add(SoundData.SoundEffectType.ENVIRONMENT, 1.0f);
            _parameters._subVolume.Add(SoundData.SoundEffectType.UI, 1.0f);
            SaveParameters();
        }

        // maybe have a _totalLevel properties, iterate on this and fullfil the _levelData propertie

        if (SerializationManager.Exist("Level_" + _currentLevel))
        {
            _levelData.Add((ObjectData)SerializationManager.Load("Level_" + _currentLevel));
        }
        else
        {
            _levelData.Add(new ObjectData());
        }

        if (SerializationManager.Exist("Powers"))
        {
            _powers = (Powers)SerializationManager.Load("Powers");
        }
        else
        {
            for (int i = 0; i < 6; ++i)
            {
                _powers._powersIndex.Add(-1);
                //_powers._powersUnlocked.Add(-1, KeyCode.None);
            }
        }

        if (SerializationManager.Exist("PowersGui"))
        {
            _guiPowers = (PowersGui)SerializationManager.Load("PowersGui");
        }
        else
        {
            for (int j = 0; j < 3; ++j)
            {
                _guiPowers._powersGuiIndex.Add(-1);
            }
        }

        if (SerializationManager.Exist("Keys"))
        {
            _nbKey = (uint)SerializationManager.Load("Keys");
        }
    }

    public void SavePowers()
    {
        SerializationManager.Save("Powers", _powers);
    }

    public Powers GetPowers()
    {
        return _powers;
    }

    public void SaveGuiPowers()
    {
        SerializationManager.Save("PowersGui", _guiPowers);
    }

    public PowersGui GetGuiPowers()
    {
        return _guiPowers;
    }

    public void SaveParameters()
    {
        SerializationManager.Save("Parameters", _parameters);
    }

    public void Save()
    {
        // save current level
        SerializationManager.Save("Level_" + _currentLevel, _levelData[_currentLevel]);
        SerializationManager.Save("Key", _nbKey);
    }

    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.S))
        //     Save();
    }
}