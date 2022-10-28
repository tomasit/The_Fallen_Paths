using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(UnityEngine.Rendering.Universal.Light2D))]
public class LightInteraction : AInteractable
{
    [SerializeField] private GameObject _objectToActive = null;
    [SerializeField] private bool _isAlight;
    [SerializeField] private float _alightDuration;
    [SerializeField] private Vector2 _intensityRange;
    [SerializeField] private Vector2 _innerRange;
    [SerializeField] private Vector2 _outerRange;
    [SerializeField] private UnityEvent<bool> _event;
    private UnityEngine.Rendering.Universal.Light2D _light;
    private Coroutine _alightCoroutine;
    private float _alightTime;
    private float _oscillationFrequence;
    private float _oscillationTimer;

    private void Start()
    {
        Load();
        _oscillationTimer = 0.0f;
        _oscillationFrequence = Random.Range(0.1f, 0.2f);
        _alightTime = 0.0f;
        _alightCoroutine = null;
        _light = GetComponent<UnityEngine.Rendering.Universal.Light2D>();
        _light.pointLightInnerRadius = _innerRange.x;
        _light.pointLightOuterRadius = _outerRange.x;
        if (_isAlight)
        {
            _light.intensity = _intensityRange.x;
            if (_objectToActive != null)
                _objectToActive.SetActive(true);
        }
        else
        {
            _light.enabled = false;
            if (_objectToActive != null)
                _objectToActive.SetActive(false);
        }
        Debug.Log("Is this light (" + gameObject.name + ") alight : " + _isAlight);
        _event.Invoke(_isAlight);
    }

    public override void Save()
    {
        SaveManager.DataInstance.ReferenceValue(GetComponent<PersistentId>().ID, nameof(_isAlight), _isAlight);
    }

    public override void Load()
    {
        if (SaveManager.DataInstance.IsReferenced(GetComponent<PersistentId>().ID, nameof(_isAlight)))
            _isAlight = (bool)SaveManager.DataInstance.GetValue(GetComponent<PersistentId>().ID, nameof(_isAlight));
    }

    private IEnumerator AlightCoroutine(float deltaTime)
    {
        while (_alightTime < _alightDuration)
        {
            if (_isAlight)
            {
                _light.intensity = _intensityRange.x * _alightTime / _alightDuration;
            }
            else
            {
                _light.intensity = _intensityRange.x - _intensityRange.x * (_alightTime / _alightDuration);
            }
            _alightTime += deltaTime;
            yield return new WaitForSeconds(deltaTime);
        }

        if (!_isAlight)
        {
            _light.enabled = false;
            if (_objectToActive != null)
                _objectToActive.SetActive(false);
        }
        _alightCoroutine = null;
    }

    private void ComputeLight()
    {
        if (_alightCoroutine != null)
            StopCoroutine(_alightCoroutine);
        
        _alightTime = 0.0f;
        if (_isAlight)
        {
            if (_objectToActive != null)
                _objectToActive.SetActive(true);
            _light.enabled = true;
        }
        _alightCoroutine = StartCoroutine(AlightCoroutine(Time.deltaTime));
    }

    public override void Interact()
    {
        _isAlight = !_isAlight;
        ComputeLight();
        _event.Invoke(_isAlight);
        Save();
    }

    private void Update()
    {
        if (_isAlight && _alightCoroutine == null)
        {
            if (_oscillationTimer >= _oscillationFrequence)
            {
                _light.intensity = Mathf.Clamp(_light.intensity + Random.Range(-.05f, .05f), _intensityRange.x, _intensityRange.y);
                _light.pointLightInnerRadius = Mathf.Clamp(_light.pointLightInnerRadius + Random.Range(-.05f, .05f), _innerRange.x, _innerRange.y);
                _light.pointLightOuterRadius = Mathf.Clamp(_light.pointLightOuterRadius + Random.Range(-.05f, .05f), _outerRange.x, _outerRange.y);
                _oscillationTimer = 0.0f;
            }
            _oscillationTimer += Time.deltaTime;
        }
    }
}
