using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractionSelectorEnable))]
public class MultiLockedInteraction : AInteractable
{
    [SerializeField] private int _unlockConditionNb;
    [SerializeField] private AInteractable[] _conditionalInteractions;

    private int _currentUnlockNb = 0;

    private void Start()
    {
        Load();
    }

    public override void Save()
    {
        SaveManager.DataInstance.ReferenceValue(GetComponent<PersistentId>().ID, nameof(_currentUnlockNb), _currentUnlockNb);
    }

    public override void Load()
    {
        if (SaveManager.DataInstance.IsReferenced(GetComponent<PersistentId>().ID, nameof(_currentUnlockNb)))
            _currentUnlockNb = (int)SaveManager.DataInstance.GetValue(GetComponent<PersistentId>().ID, nameof(_currentUnlockNb));
    }

    public override void Interact()
    {
        _currentUnlockNb += 1;
        if (_currentUnlockNb == _unlockConditionNb)
        {
            GetComponent<InteractionSelectorEnable>().Interact();
            foreach (var interaction in _conditionalInteractions)
                interaction.Interact();
        }
        Save();
    }
}
