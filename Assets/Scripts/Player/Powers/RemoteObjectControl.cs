using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteObjectControl : MonoBehaviour
{
    [SerializeField] private Transform _playerParticleParent;
    [SerializeField] private GameObject _particlePrefab;
    [SerializeField] private float _particleXOffset;
    [SerializeField] private float _powerDuration;
    [SerializeField] private float _triggerDistance;
    private RemoteControllableObject[] _triggerableObjects;
    private List<GameObject> _instantiatedParticles;
    private float _durationCounter;
    private Coroutine _powerCoroutine = null;
    private RemoteControllableObject _objectInTouch = null;

    private void Start()
    {
        _triggerableObjects = FindObjectsOfType<RemoteControllableObject>(true);
        _durationCounter = 0.0f;
        _instantiatedParticles = new List<GameObject>();
    }

    public bool CanUse()
    {
        return _powerCoroutine == null ? true : false;
    }

    public void Use()
    {
        _powerCoroutine = StartCoroutine(RemoteObjectCoroutine());
    }

    public void Fire()
    {

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
    }

    private void DestroyParticles()
    {
        foreach (var particle in _instantiatedParticles)
            Destroy(particle);
        _instantiatedParticles.Clear();
    }

    private void CheckObjectInTouch()
    {
        if (_objectInTouch != null)
            _objectInTouch.ActiveOutline(false);
    }

    private void CheckCollision()
    {
        RaycastHit2D hit;
        if ((hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, float.PositiveInfinity)).collider != null)
        {
            var remote = hit.collider.gameObject.GetComponent<RemoteControllableObject>();
            if (remote != null)
            {
                if (GetDistance(hit.collider.transform.position) <= _triggerDistance)
                {
                    if (_objectInTouch != null && _objectInTouch != remote)
                        _objectInTouch.ActiveOutline(false);
                    _objectInTouch = remote;
                    _objectInTouch.ActiveOutline(true);
                }
                else
                {
                    CheckObjectInTouch();
                }
            }
            else
            {
                CheckObjectInTouch();
            }
        }
        else
        {
            CheckObjectInTouch();
        }
    }

    private float GetDistance(Vector3 position)
    {
        return Vector2.Distance(position, transform.position);
    }

    private void CheckDistance()
    {
        foreach (var triggerable in _triggerableObjects)
        {
            if (!triggerable.transform.gameObject.activeSelf)
                continue;
            if (GetDistance(triggerable.transform.position) <= _triggerDistance)
            {
                triggerable.RateUpParticle();
            }
            else
            {
                triggerable.RateDownParticle();
            }
        }
    }

    private void UnactiveRemoteObjectParticle()
    {
        foreach (var triggerable in _triggerableObjects)
        {
            triggerable.UnactiveParticle();
        }
    }


    private IEnumerator RemoteObjectCoroutine()
    {
        SetupParticles();

        while (_durationCounter < _powerDuration)
        {
            CheckCollision();
            CheckDistance();
            _durationCounter += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        DestroyParticles();
        UnactiveRemoteObjectParticle();
        if (_objectInTouch != null)
            _objectInTouch.ActiveOutline(false);

        _objectInTouch = null;
        _durationCounter = 0.0f;
        _powerCoroutine = null;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (CanUse())
                Use();
        }
    }
}