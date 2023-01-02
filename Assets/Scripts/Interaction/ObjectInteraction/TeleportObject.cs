using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TeleportObject : AInteractable
{
    [SerializeField] private GameObject _objectToTeleport;
    [SerializeField] private Transform _positionToTeleport;
    [SerializeField] private float _delay = 0f;
    [SerializeField] public SoundData.SoundEffectName _soundType;
    private SoundEffect _soundEffectPlayer;

    private void Awake()
    {
        _soundEffectPlayer = GetComponent<SoundEffect>();
    }

    public override void Interact()
    {
        if (_delay <= 0f) {
            _objectToTeleport.transform.position = _positionToTeleport.position;
            return;
        }
        StartCoroutine(ComputeInteraction());
    }

    private IEnumerator ComputeInteraction()
    {
        if (_soundEffectPlayer != null)
            _soundEffectPlayer.PlaySound(_soundType);
        _objectToTeleport.SetActive(false);
        yield return new WaitForSeconds(_delay);
        _objectToTeleport.SetActive(true);
        _objectToTeleport.transform.position = _positionToTeleport.position;
    }

    public override void Save()
    {
    }

    public override void Load()
    {
    }
}
