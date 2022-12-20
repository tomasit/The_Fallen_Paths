using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteObjectControl : ARangedPower
{
    [SerializeField] private Transform _playerParticleParent;
    [SerializeField] private GameObject _particlePrefab;
    [SerializeField] private float _particleXOffset;
    private RemoteControllableObject[] _triggerableObjects;
    private List<GameObject> _instantiatedParticles;
    private RemoteControllableObject _objectInTouch = null;

    protected override void Start()
    {
        base.Start();
        _triggerableObjects = FindObjectsOfType<RemoteControllableObject>(true);
        _instantiatedParticles = new List<GameObject>();
    }

    public override void Use()
    {
        activated = true;
        SetupParticles();
    }

    private void SetupParticles()
    {
        _instantiatedParticles.Add(Instantiate(_particlePrefab, _playerParticleParent));
        _instantiatedParticles.Add(Instantiate(_particlePrefab, _playerParticleParent));

        var pos1 = _instantiatedParticles[0].transform.localPosition;
        var pos2 = _instantiatedParticles[1].transform.localPosition;
        pos1.x -= _particleXOffset;
        pos2.x += _particleXOffset;

        _instantiatedParticles[0].transform.localPosition = pos1;
        _instantiatedParticles[1].transform.localPosition = pos2;

        var shape1 = _instantiatedParticles[0].GetComponent<ParticleSystem>().shape;
        var shape2 = _instantiatedParticles[1].GetComponent<ParticleSystem>().shape;
        shape1.rotation = new Vector3(0.0f, -90.0f, 0.0f);
        shape1.alignToDirection = true;
        shape2.rotation = new Vector3(0.0f, 90.0f, 0.0f);
        shape2.alignToDirection = false;

        // active all trigerable particles
        foreach (var triggerable in _triggerableObjects)
        {
            if (!triggerable.transform.gameObject.activeSelf)
                continue;
            triggerable.RateUpParticle();
        }
    }

    public override void CancelRange()
    {
        base.CancelRange();
        DestroyParticles();
        UnactiveRemoteObjectParticle();
        UnPreview();
        _objectInTouch = null;
    }

    public override void Cancel()
    {
        return;
    }

    private void DestroyParticles()
    {
        foreach (var particle in _instantiatedParticles)
            Destroy(particle);
        _instantiatedParticles.Clear();
    }

    public bool HasObjectInRange()
    {
        foreach (var triggerable in _triggerableObjects)
        {
            if (!triggerable.transform.gameObject.activeSelf)
                continue;
            if (Vector2.Distance(triggerable.transform.position, transform.position) <= rangeRadius)
            {
                return true;
            }
        }
        return false;
    }

    private void UnactiveRemoteObjectParticle()
    {
        foreach (var triggerable in _triggerableObjects)
        {
            triggerable.UnactiveParticle();
        }
    }

    protected override void Preview()
    {
        if (_objectInTouch != null)
            _objectInTouch.ActiveOutline(true);
    }

    protected override void UnPreview()
    {
        if (_objectInTouch != null)
            _objectInTouch.ActiveOutline(false);
    }

    protected override bool canCastPower()
    {
        RaycastHit2D hit;

        if ((hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, float.PositiveInfinity)).collider != null)
        {
            var remote = hit.collider.gameObject.GetComponent<RemoteControllableObject>();
            if (remote != null && remote.IsTriggerable())
            {
                if (_objectInTouch != null && _objectInTouch != remote)
                    UnPreview();
                _objectInTouch = remote;
                return true;
            }
        }
        return false;
    }

    public override void Fire()
    {
        _powerManager.ActivatePowerCooldownFromStackTrace();
        _objectInTouch.Trigger();
    }

    private void Update()
    {
        if (activated)
        {
            // CheckDistance();
            if (mouseDistranceIsCorrect())
            {
                if (canCastPower())
                {
                    Preview();
                    if (Input.GetMouseButtonDown(0))
                    {
                        Fire();
                        CancelRange();
                    }
                }
                else
                    UnPreview();
            }
            else
                UnPreview();
        }
    }
}
