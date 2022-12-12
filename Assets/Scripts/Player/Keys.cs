using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keys : MonoBehaviour
{
    [SerializeField] private uint nbKeys = 0;

    public uint GetNbKeys()
    {
        return nbKeys;
    }
    
    public void AddKeys(uint keysToAdd)
    {
        nbKeys += keysToAdd;
    }

    public void RemoveKeys(uint keysToRemove)
    {
        nbKeys -= keysToRemove;
    }
}
