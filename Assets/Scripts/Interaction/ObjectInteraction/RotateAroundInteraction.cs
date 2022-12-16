using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundInteraction : AInteractable
{
    [SerializeField] private Transform _rotateAround;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private bool _untriggerWhenRotate;
    [SerializeField] private Vector2 _rotationRange;
    private bool _hasRotated = false;
    private bool _isRotating = false;

    private void Start()
    {
        Load();
    }

    public override void Save()
    {
        // SaveManager.DataInstance.ReferenceValue(GetComponent<PersistentId>().ID, nameof(_hasRotated), _hasRotated);
    }

    public override void Load()
    {
        // if (SaveManager.DataInstance.IsReferenced(GetComponent<PersistentId>().ID, nameof(_hasRotated)))
        //     _hasRotated = (bool)SaveManager.DataInstance.GetValue(GetComponent<PersistentId>().ID, nameof(_hasRotated));
    }

    public override void Interact()
    {
        _hasRotated = !_hasRotated;
        _isRotating = true;
        GetComponent<Collider2D>().isTrigger = _untriggerWhenRotate;
        Save();
    }

    private void Update()
    {
        if (_isRotating)
        {
            transform.RotateAround(_rotateAround.position, _hasRotated ? Vector3.forward : Vector3.back, _rotationSpeed * Time.deltaTime);
            var rotation = transform.eulerAngles;
            rotation.z = (rotation.z > 180.0f) ? rotation.z - 360.0f : rotation.z;
            rotation.z = Mathf.Clamp(rotation.z, _rotationRange.x, _rotationRange.y);
            transform.eulerAngles = rotation;

            if ((_hasRotated && rotation.z == _rotationRange.y) || (!_hasRotated && rotation.z == _rotationRange.x))
            {
                _isRotating = false;
                GetComponent<Collider2D>().isTrigger = false;
            }
        }
    }
}
