using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoStateInteraction : AInteractable
{
    [SerializeField] private SoundData.SoundEffectName[] _soundNames;
    [SerializeField] private string[] _description;
    [SerializeField] private Sprite[] _stateSprite;
    private int _spriteIndex = 0;
    private SoundEffect _soundEffectPlayer = null;

    private void Start()
    {
        _soundEffectPlayer = GetComponent<SoundEffect>();
        Load();
        _descriptionHeight = AInteractable.DescriptionHeight.TWO_STATE_DESCRIPTION;
        GetComponent<SpriteRenderer>().sprite = _stateSprite[_spriteIndex];
    }

    public override void Save()
    {
        SaveManager.DataInstance.ReferenceValue(GetComponent<PersistentId>().ID, nameof(_spriteIndex), _spriteIndex);
    }

    public override void Load()
    {
        if (SaveManager.DataInstance.IsReferenced(GetComponent<PersistentId>().ID, nameof(_spriteIndex)))
            _spriteIndex = (int)SaveManager.DataInstance.GetValue(GetComponent<PersistentId>().ID, nameof(_spriteIndex));
    }

    public override string GetDescription()
    {
        return _description[_spriteIndex];
    }

    public override void Interact()
    {
        _spriteIndex = (_spriteIndex == 0 ? 1 : 0);
        GetComponent<SpriteRenderer>().sprite = _stateSprite[_spriteIndex];
        Save();
        if (_soundEffectPlayer != null)
            _soundEffectPlayer.PlaySound(_soundNames[_spriteIndex]);
    }
}
