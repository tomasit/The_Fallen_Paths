using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractionProcessor))]
public class InteractionSelectorEnable : AInteractable
{
    private bool _unlock = false;
    [ColorUsageAttribute(true, true)][SerializeField] private Color _lockedColor;
    [SerializeField] private string _description;

    public void Start()
    {
        Load();
        _descriptionHeight = (_unlock ? DescriptionHeight.NONE : DescriptionHeight.LOCK);
        GetComponent<InteractionProcessor>()._enabled = _unlock;
        if (!_unlock)
        {
            GetComponent<GlowOnTouch>().SetOutlineColor(_lockedColor, false);
        }
    }

    public override string GetDescription()
    {
        return _description;
    }

    public override void Save()
    {
        // SaveManager.DataInstance.ReferenceValue(GetComponent<PersistentId>().ID, nameof(_unlock), _unlock);
    }

    public override void Load()
    {
        // if (SaveManager.DataInstance.IsReferenced(GetComponent<PersistentId>().ID, nameof(_unlock)))
        //     _unlock = (bool)SaveManager.DataInstance.GetValue(GetComponent<PersistentId>().ID, nameof(_unlock));
    }

    public override void Interact()
    {
        _unlock = !_unlock;
        GetComponent<InteractionProcessor>()._enabled = _unlock;
        GetComponent<GlowOnTouch>().SetOutlineColor(_lockedColor, _unlock);
        _descriptionHeight = _unlock ? AInteractable.DescriptionHeight.NONE : DescriptionHeight.LOCK;
        Save();
    }
}
