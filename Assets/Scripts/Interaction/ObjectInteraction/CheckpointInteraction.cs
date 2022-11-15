using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(TMPDialogue))]
public class CheckpointInteraction : AInteractable
{
    [SerializeField] private GameObject _fire;
    [SerializeField] private GameObject[] _particles;
    private TMPDialogue _dialogue;
    private bool _lastActivated = false;

    private void Start()
    {
        _dialogue = GetComponent<TMPDialogue>();
    }

    public override void Interact()
    {
        _lastActivated = true;
        // mettre toutes les autres statues a false;
        if (!_fire.activeSelf)
            _fire.SetActive(true);
        foreach (var p in _particles)
        {
            if (!p.activeSelf)
                p.SetActive(true);
        }
    }

    public override void Load()
    {
        // woula flemme pour le moment
    }

    public override void Save()
    {
        // woula flemme pour le moment
    }
}
