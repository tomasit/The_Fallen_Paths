using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyLockedInteraction : AInteractable
{
    [SerializeField] private Keys keysManager;
    [SerializeField] private bool _isLocked = true;

    void Start()
    {
        Load();
    }

    public override void Save()
    {
        SaveManager.DataInstance.ReferenceValue(GetComponent<PersistentId>().ID, nameof(_isLocked), _isLocked);
    }

    public override void Load()
    {
        if (SaveManager.DataInstance.IsReferenced(GetComponent<PersistentId>().ID, nameof(_isLocked)))
            _isLocked = (bool)SaveManager.DataInstance.GetValue(GetComponent<PersistentId>().ID, nameof(_isLocked));
    }

    public override void Interact()
    {
        if (!_isLocked)
            return;
        if (GetComponent<InteractionProcessor>().IsAvailable())
        {
            if (keysManager.GetNbKeys() >= 1) {
                keysManager.RemoveKeys(1);
            }
            _isLocked = false;
            Save();
        }
        //     //unlock sound
        //     if (_soundEffectPlayer != null) {
        //         //Debug.Log("Play le son normalement");
        //         _soundEffectPlayer.PlaySound(SoundData.SoundEffectName.INTERACTION_CHEST);
        //     } else {
        //         //Debug.Log("Sound effect == null");
        //     }
        //     _objectActiveFalseOnUnlock.enabled = false;
        //     foreach (var interaction in _unlockInteractions) {
        //         interaction.Interact();
        //     }
        //     Save();
        // } else {
        //     //locked sound
        //     if (_soundEffectPlayer != null) {
        //         //Debug.Log("Play le son normalement");
        //         _soundEffectPlayer.PlaySound(SoundData.SoundEffectName.INTERACTION_CHEST);
        //     }  else {
        //         //Debug.Log("Sound effect == null");
        //     }
        // }
    }
}