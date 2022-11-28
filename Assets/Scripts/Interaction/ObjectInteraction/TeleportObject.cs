using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TeleportObject : AInteractable
{
    [SerializeField] private GameObject _objectToTeleport;
    [SerializeField] private Transform _positionToTeleport;

    public override void Interact()
    {
        _objectToTeleport.transform.position = _positionToTeleport.position;
    }

    public override void Save()
    {
    }

    public override void Load()
    {
    }
}
