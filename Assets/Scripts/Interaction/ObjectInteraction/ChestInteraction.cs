using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestInteraction : AInteractable
{
    private SoundEffect _soundEffectPlayer = null;
    [SerializeField] private bool _isOpen;
    
    [SerializeField] private uint _nbKeyContaining;

    [SerializeField] private SpriteRenderer open;
    [SerializeField] private SpriteRenderer closed;
    private SpriteRenderer _currentSprite;

    [SerializeField] AInteractable [] _openInteraction;

    private void Start()
    {
        Load();
        _soundEffectPlayer = GetComponent<SoundEffect>();
        _currentSprite = GetComponent<SpriteRenderer>();
        _currentSprite.sprite = _isOpen ? open.sprite : closed.sprite;
        transform.position = _isOpen ? open.gameObject.transform.position : closed.gameObject.transform.position;
    }

    public override void Save()
    {
        // SaveManager.DataInstance.ReferenceValue(GetComponent<PersistentId>().ID, nameof(_isOpen), _isOpen);
    }

    public override void Load()
    {
        // if (SaveManager.DataInstance.IsReferenced(GetComponent<PersistentId>().ID, nameof(_isOpen)))
        //     _isOpen = (bool)SaveManager.DataInstance.GetValue(GetComponent<PersistentId>().ID, nameof(_isOpen));
    }

    public override void Interact()
    {
        if (!_isOpen) {
            _isOpen = true;
            if (_soundEffectPlayer != null)
                _soundEffectPlayer.PlaySound(SoundData.SoundEffectName.INTERACTION_CHEST);
            _currentSprite.sprite = _isOpen ? open.sprite : closed.sprite;
            transform.position = _isOpen ? open.gameObject.transform.position : closed.gameObject.transform.position;
            
            //interact le tableau d'interaction
            foreach (var interaction in _openInteraction) {
                interaction.Interact();
            }
        }
        Save();
    }
}