using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoStateInteraction : AInteractable
{
    [SerializeField] private string[] _description;
    [SerializeField] private Sprite[] _stateSprite;
    private int _spriteIndex = 0;

    private void Start()
    {
        Load();
        _descriptionHeight = AInteractable.DescriptionHeight.TWO_STATE_DESCRIPTION;
        GetComponent<SpriteRenderer>().sprite = _stateSprite[_spriteIndex];
        // var triggerProcessor = GetComponent<CollisionDetection>();
        // if (triggerProcessor != null && GetComponent<InteractionProcessor>()._enabled)
        // {
        //     triggerProcessor._displayedText = _description[_spriteIndex];
        // }
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
        // var triggerProcessor = GetComponent<CollisionDetection>();
        // if (triggerProcessor != null)
        // {
        //     triggerProcessor._displayedText = _description[_spriteIndex];
        //     triggerProcessor.SetDescription(_description[_spriteIndex]);
        // }
    }
}
