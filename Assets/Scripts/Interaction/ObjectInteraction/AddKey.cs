using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddKey : AInteractable
{
    [SerializeField] private uint _nbKeyToAdd;
    [SerializeField] private Keys keysManager;

    public override void Save()
    {
    }

    public override void Load()
    {
    }

    public override void Interact()
    {
        if (keysManager != null) {
            keysManager.AddKeys(_nbKeyToAdd);
        }
    }
}