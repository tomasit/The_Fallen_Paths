using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RoomLightInteraction : AInteractable
{
    [SerializeField] private bool _isAlight;
    [SerializeField] private float _intensity = 0.5f;
    [SerializeField] private float _timeToWaitEachFrame = 0.1f;
    [SerializeField] private float _intensityToAddEachTimeToWait = 0.025f;
    [SerializeField] private bool externalLight;
    [SerializeField] private Light2D _light;

    private void Start()
    {
        Load();
        if (!externalLight) {
            _light = GetComponent<Light2D>();
        }
        if (_light != null) {
            _light.intensity = _isAlight ? _intensity : 0f;
        }
    }

    public override void Save()
    {
        // SaveManager.DataInstance.ReferenceValue(GetComponent<PersistentId>().ID, nameof(_isAlight), _isAlight);
    }

    public override void Load()
    {
        // if (SaveManager.DataInstance.IsReferenced(GetComponent<PersistentId>().ID, nameof(_isAlight)))
        //     _isAlight = (bool)SaveManager.DataInstance.GetValue(GetComponent<PersistentId>().ID, nameof(_isAlight));
    }

    public override void Interact()
    {
        if (!_isAlight) {
            _isAlight = true;
            if (_light != null) {
                StartCoroutine(AlightInTime());
            }
        }
        Save();
    }

    private IEnumerator AlightInTime()
    {
        while(_light.intensity <= _intensity) {
            _light.intensity += _intensityToAddEachTimeToWait;
            yield return new WaitForSeconds(_timeToWaitEachFrame);
        }
        _light.intensity = _intensity;
    }
}
