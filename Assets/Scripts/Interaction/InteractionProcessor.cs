using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TriggerProcessor))]
public class InteractionProcessor : AInteractable
{
    [SerializeField] private AInteractable[] _interactions;
    [SerializeField] private bool _stopOnInteract;
    [HideInInspector] public bool _enabled = true;
    [HideInInspector] public bool _interact = false;
    private TriggerProcessor _triggerInteractor = null;
    private SoundEffect _soundEffectPlayer = null;

    private void Start()
    {
        Load();
        _soundEffectPlayer = GetComponent<SoundEffect>();
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

    public bool IsAvailable()
    {
        return (!_interact && _enabled) ? true : false;
    }

    public override void Interact()
    {
        if (_interact)
            return;

        if (!_enabled)
        {
            _soundEffectPlayer.PlaySound(SoundData.SoundEffectName.INTERACTION_LOCK);
            return;
        }

        foreach (var interaction in _interactions)
        {
            //Debug.Log("Interact");
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
        // SaveManager.DataInstance.ReferenceValue(GetComponent<PersistentId>().ID, nameof(_interact), _interact);
    }

    public override void Load()
    {
        // if (SaveManager.DataInstance.IsReferenced(GetComponent<PersistentId>().ID, nameof(_interact)))
        //     _interact = (bool)SaveManager.DataInstance.GetValue(GetComponent<PersistentId>().ID, nameof(_interact));
    }
}
