using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keys : MonoBehaviour
{
    [SerializeField] private uint _nbKey;
    //soundManager !

    private void Start()
    {
        Load();
    }

    public uint GetNbKeys()
    {
        return _nbKey;
    }
    
    public void AddKeys(uint keysToAdd)
    {
        _nbKey += keysToAdd;
        Save();
    }

    public void RemoveKeys(uint keysToRemove)
    {
        _nbKey -= keysToRemove;
        Save();
    }

    public void Save()
    {
        SaveManager.DataInstance.SetNbKey(_nbKey);
        //SaveManager.DataInstance.SaveKeys();
    }

    public void Load()
    {
        Debug.Log("Nb keys = " + _nbKey);
        _nbKey = SaveManager.DataInstance.GetNbKey();
    }
}
