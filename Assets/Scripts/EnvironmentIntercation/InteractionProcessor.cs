using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TriggerProcessor))]
public class InteractionProcessor : AInteractable
{
    [SerializeField] private AInteractable[] _interactions;
    [SerializeField] private bool _stopOnInteract;
    [HideInInspector] public bool _enabled = true;
    private bool _interact = false;
    private TriggerProcessor _triggerInteractor = null;

    private void Start()
    {
        Load();
        _triggerInteractor = GetComponent<TriggerProcessor>();
        if (_interact)
            DisableTriggerInteractor();
    }

    private void DisableTriggerInteractor()
    {
        if (_triggerInteractor is CollisionDetection)
            GetComponent<GlowOnTouch>().Trigger(false);
        _triggerInteractor.DisableTrigger();
    }

    public override void Interact()
    {
        if (_interact || !_enabled)
            return;

        foreach (var interaction in _interactions)
        {
            interaction.Interact();
        }

        if (_stopOnInteract)
        {
            _interact = true;
            DisableTriggerInteractor();
        }

        Save();
    }

    public override void Save()
    {
        SaveManager.DataInstance.ReferenceValue(GetComponent<PersistentId>().ID, nameof(_interact), _interact);
    }

    public override void Load()
    {
        if (SaveManager.DataInstance.IsReferenced(GetComponent<PersistentId>().ID, nameof(_interact)))
            _interact = (bool)SaveManager.DataInstance.GetValue(GetComponent<PersistentId>().ID, nameof(_interact));
    }
}
