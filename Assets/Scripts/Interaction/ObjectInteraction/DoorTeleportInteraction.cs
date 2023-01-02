using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public struct SpriteChange {
    public SpriteRenderer spriteToChange;
    public Sprite [] spriteArray;
    public float delayTrigger;
    public float delayReset;
    public SoundData.SoundEffectName soundType;
};

public class DoorTeleportInteraction : AInteractable
{
    [SerializeField] private GameObject _objectToTeleport;
    [SerializeField] private Transform _positionToTeleport;
    [SerializeField] private float _delay = 0f;
    private SoundEffect _soundEffectPlayer;
    [SerializeField] private SpriteChange [] _spritesToChange;

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

        foreach (var spriteChange in _spritesToChange) {
            if (spriteChange.spriteArray.Length == 2) {
                StartCoroutine(ChangeSprites(spriteChange));
            }
        }
    }

    private IEnumerator ChangeSprites(SpriteChange spriteManager)
    {
        yield return new WaitForSeconds(spriteManager.delayTrigger);

        spriteManager.spriteToChange.sprite = spriteManager.spriteArray[1];
        if (_soundEffectPlayer != null)
            _soundEffectPlayer.PlaySound(spriteManager.soundType);

        yield return new WaitForSeconds(spriteManager.delayReset);

        spriteManager.spriteToChange.sprite = spriteManager.spriteArray[0];
    }

    private IEnumerator ComputeInteraction()
    {
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
