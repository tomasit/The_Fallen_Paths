using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DestroyInteraction : AInteractable
{
    [SerializeField] private CameraShake _cameraShake;
    [SerializeField] private float _maxShakeAmount;
    [SerializeField] private int _numberTap;
    [SerializeField] private Sprite[] _spriteSheet;
    [SerializeField] private Behaviour[] _componentToDesactivate;
    private SoundEffect _soundEffectPlayer;
    private SpriteRenderer _sprite;
    private int _currentTap;
    private Vector3 _originalPos;

    private void Start()
    {
        _soundEffectPlayer = GetComponent<SoundEffect>();
        _currentTap = 0;
        _sprite = GetComponent<SpriteRenderer>();
    }

    public bool IsObjectDestroy()
    {
        return (_currentTap >= _numberTap);
    }

    public override void Interact()
    {
        _soundEffectPlayer.PlaySound(SoundData.SoundEffectName.INTERACTION_BREAK_METAL);
        _currentTap += 1;
        float percentage = (float)_currentTap / (float)_numberTap;
        _originalPos = (_currentTap == 1 ? _cameraShake.transform.localPosition : _originalPos);
        _cameraShake.SetValues(_maxShakeAmount * percentage, _originalPos);
        int newIndex = (int)(_spriteSheet.Length * percentage);
        newIndex = (newIndex == _spriteSheet.Length - 1 && percentage < 1.0f ? newIndex -= 1 : newIndex);
        _sprite.sprite = _spriteSheet[Mathf.Clamp(newIndex, 0, _spriteSheet.Length - 1)];

        if (_currentTap == _numberTap)
        {
            foreach (var component in _componentToDesactivate)
            {
                component.enabled = false;
            }
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
