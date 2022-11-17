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
    }

    public void Reset()
    {
        _activated = false;

        if (_fire.activeSelf)
            _fire.SetActive(false);
        foreach (var p in _particles)
        {
            var emission = p._particle.GetComponent<ParticleSystem>().emission;
            emission.rateOverTime = p._rateOverTime;
        }
        Save();
    }

    public override void Interact()
    {
        _activated = true;
        _dialogue.StartDialogue((_activated ? "ActivatedInteraction" : "ActiveInteraction"));

        // foreach (var checkPoint in _checkpoints)
        // {
        //     if (checkPoint.transform.gameObject != transform.gameObject)
        //         checkPoint.Reset();
        // }

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
    }

    public override void Save()
    {
        SaveManager.DataInstance.ReferenceValue(GetComponent<PersistentId>().ID, nameof(_activated), _activated);
    }

    private void Update()
    {
        if (_outline == null)
            return;
        if (!_outline.IsTrigger() && !_dialogue.IsFinish())
            _dialogue.StopDialogue();
        else if (_outline.IsTrigger() && _dialogue.IsFinish())
            _dialogue.StartDialogue((_activated ? "ActivatedInteraction" : "ActiveInteraction"));
    }
}
