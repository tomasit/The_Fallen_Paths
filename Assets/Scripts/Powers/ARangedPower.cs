using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ARangedPower : APower
{
    public float rangeRadius = 0;
    public bool showingRange = false;

    [SerializeField] private ParticleSystem _rangePrefab = null;
    private ParticleSystem _rangeObject = null;

    [SerializeField] private Color _rangeColor = Color.white;
    // Update is called once per frame
    bool playerClickIsCorrect()
    {
        return false; // PLACEHOLDER
    }

    public override void Use()
    {
        showingRange = true;
        showRange();
    }

    void showRange()
    {
        _rangeObject = Instantiate(_rangePrefab, transform);

        var rangeShape = _rangeObject.shape;
        rangeShape.radius = rangeRadius;

        var rangeMain = _rangeObject.main;
        rangeMain.startColor = _rangeColor;

        _rangeObject.Play();
    }

    void Update()
    {
        // NOTE: KeyCodes for debug. R = Activate Range && A = Use Power
        if (showingRange)
        {
            if (Input.GetKey(KeyCode.A) || playerClickIsCorrect())
            {
                _rangeObject.Stop();
                showingRange = false;
                Fire();
            }

        }
        else if (Input.GetKey(KeyCode.R))
        {
            Use();
        }
        else if (_rangeObject != null && _rangeObject.particleCount == 0)
        {
            Destroy(_rangeObject.gameObject);
            _rangeObject = null;
        }
    }
}
