using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyLockedInteraction : AInteractable
{
    private SoundEffect _soundEffectPlayer = null;
    [SerializeField] private bool _isLocked;
    [SerializeField] private Keys keysManager;

    [SerializeField] private AInteractable _unlockInteraction;
    [SerializeField] private SpriteRenderer _objectActiveFalseOnUnlock;

    void Start()
    {
        Load();
        _soundEffectPlayer = GetComponent<SoundEffect>();
        if (!_isLocked) {
            _objectActiveFalseOnUnlock.enabled = false;
        }
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
        Debug.Log("keysManager.GetNbKeys() = " + keysManager.GetNbKeys());
        if (keysManager.GetNbKeys() >= 1) {
            _isLocked = false;
            keysManager.RemoveKeys(1);
            //unlock sound
            if (_soundEffectPlayer != null) {
                //Debug.Log("Play le son normalement");
                _soundEffectPlayer.PlaySound(SoundData.SoundEffectName.INTERACTION_CHEST);
            } else {
                //Debug.Log("Sound effect == null");
            }
            _objectActiveFalseOnUnlock.enabled = false;
            _unlockInteraction.Interact();
            Save();
        } else {
            //locked sound
            if (_soundEffectPlayer != null) {
                //Debug.Log("Play le son normalement");
                _soundEffectPlayer.PlaySound(SoundData.SoundEffectName.INTERACTION_CHEST);
            }  else {
                //Debug.Log("Sound effect == null");
            }
        }
    }
}