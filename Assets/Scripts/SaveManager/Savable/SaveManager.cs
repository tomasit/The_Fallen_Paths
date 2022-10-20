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
    private int _currentLevel = 0;

    private void Awake() {
        Debug.Log("Singleton awake");
        if (_dataInstance != null) {
            Debug.Log("Singleton always present on the scene, destroy this one");
            Destroy(this);
        } else {
            DontDestroyOnLoad(this);
        }
        _levelData = new List<ObjectData>();
        Load();
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
        // maybe have a _totalLevel properties, iterate on this and fullfil the _levelData propertie

        if (SerializationManager.Exist("Level_" + _currentLevel))
        {
            _levelData.Add((ObjectData)SerializationManager.Load("Level_" + _currentLevel));
        }
        else
        {
            _levelData.Add(new ObjectData());
        }
    }

    public void Save()
    {
        SerializationManager.Save("Level_" + _currentLevel, _levelData[_currentLevel]);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
            Save();
    }
}