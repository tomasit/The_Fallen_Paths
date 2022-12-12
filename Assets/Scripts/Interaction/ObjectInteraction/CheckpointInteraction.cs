using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(TMPDialogue))]
public class CheckpointInteraction : AInteractable
{
    [System.Serializable]
    public struct EmissionParticle
    {
        public GameObject _particle;
        public int _rateOverTime;
    }
    [SerializeField] private GameObject _fire;
    [SerializeField] private EmissionParticle[] _particles;
    private TMPDialogue _dialogue;
    private bool _activated = false;
    private GlowOnTouch _outline = null;
    private bool _choosenByUser = false;
    private CheckpointInteraction[] _checkpoints;

    private void Start()
    {
        _checkpoints = FindObjectsOfType<CheckpointInteraction>();

        Load();
        _outline = GetComponent<GlowOnTouch>();
        _fire.SetActive(_activated);
        foreach (var p in _particles)
        {
            var emission = p._particle.GetComponent<ParticleSystem>().emission;
            emission.rateOverTime = (_activated ? 0 : p._rateOverTime);
        }
        _dialogue = GetComponent<TMPDialogue>();

        if (_choosenByUser)
            FindObjectOfType<PlayerController>().transform.position = transform.position;
    }

    public void Reset()
    {
        // _activated = false;
        _choosenByUser = false;
        // if (_fire.activeSelf)
        //     _fire.SetActive(false);
        // foreach (var p in _particles)
        // {
        //     var emission = p._particle.GetComponent<ParticleSystem>().emission;
        //     emission.rateOverTime = p._rateOverTime;
        // }
        Save();
    }

    public override void Interact()
    {
        _activated = true;
        _choosenByUser = true;
        _dialogue.StartDialogue("ChooseByUserCheckpoint");

        foreach (var checkPoint in _checkpoints)
        {
            if (checkPoint.transform.gameObject != transform.gameObject)
                checkPoint.Reset();
        }

        if (!_fire.activeSelf)
            _fire.SetActive(true);

        foreach (var p in _particles)
        {
            var emission = p._particle.GetComponent<ParticleSystem>().emission;
            emission.rateOverTime = (_activated ? 0 : p._rateOverTime);
        }
        Save();
    }

    public override void Load()
    {
        if (SaveManager.DataInstance.IsReferenced(GetComponent<PersistentId>().ID, nameof(_activated)))
            _activated = (bool)SaveManager.DataInstance.GetValue(GetComponent<PersistentId>().ID, nameof(_activated));
        if (SaveManager.DataInstance.IsReferenced(GetComponent<PersistentId>().ID, nameof(_choosenByUser)))
            _choosenByUser = (bool)SaveManager.DataInstance.GetValue(GetComponent<PersistentId>().ID, nameof(_choosenByUser));
    }

    public override void Save()
    {
        SaveManager.DataInstance.ReferenceValue(GetComponent<PersistentId>().ID, nameof(_activated), _activated);
        SaveManager.DataInstance.ReferenceValue(GetComponent<PersistentId>().ID, nameof(_choosenByUser), _choosenByUser);
    }

    private void Update()
    {
        if (_outline == null)
            return;
        if (!_outline.IsTrigger() && !_dialogue.IsFinish())
            _dialogue.StopDialogue();
        else if (_outline.IsTrigger() && _dialogue.IsFinish())
        {
            if (!_activated)
                _dialogue.StartDialogue("UnactiveCheckpoint");
            else if (_activated && !_choosenByUser)
                _dialogue.StartDialogue("ActiveCheckpoint");
            else
                _dialogue.StartDialogue("ChooseByUserCheckpoint");
        }
    }
}