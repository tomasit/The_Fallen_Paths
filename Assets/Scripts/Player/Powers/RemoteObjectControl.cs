using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteObjectControl : MonoBehaviour
{
    [SerializeField] private Transform _playerParticleParent;
    [SerializeField] private GameObject _particlePrefab;
    [SerializeField] private float _particleXOffset;
    [SerializeField] private float _powerDuration;
    private List<GameObject> _instantiatedParticles;
    private float _durationCounter;
    private Coroutine _powerCoroutine = null;

    private void Start()
    {
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

    private IEnumerator RemoteObjectCoroutine()
    {
        SetupParticles();

        while (_durationCounter < _powerDuration)
        {
            _durationCounter += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        DestroyParticles();
        _durationCounter = 0.0f;
        _powerCoroutine = null;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log(CanUse());
            if (CanUse())
                Use();
        }
    }
}