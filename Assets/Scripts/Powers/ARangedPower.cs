using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ARangedPower : APower
{
    public float rangeRadius = 0;
    public bool showingRange = false;

    [SerializeField] private ParticleSystem _rangePrefab = null;
    private ParticleSystem _rangeObject = null;

    [SerializeField] private ParticleSystem.MinMaxGradient _rangeColor = Color.white;
    // Update is called once per frame
    protected bool mouseDistranceIsCorrect()
    {
        Vector2 distance = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        return distance.magnitude <= (rangeRadius * _rangeObject.transform.localScale.x); // PLACEHOLDER

    }

    public override void Use()
    {
        showingRange = true;
        showRange();
    }

    protected void showRange()
    {
        _rangeObject = Instantiate(_rangePrefab, transform);

        var rangeShape = _rangeObject.shape;
        rangeShape.radius = rangeRadius;

        var rangeMain = _rangeObject.main;
        rangeMain.startColor = _rangeColor;

        _rangeObject.Play();
    }

    protected abstract void Preview();

    void Update()
    {
        // NOTE: KeyCodes for debug. R = Activate Range && A = Use Power
        if (showingRange)
        {
            if (mouseDistranceIsCorrect())
            {
                Preview();
                if (Input.GetMouseButtonDown(0))
                {
                    _rangeObject.Stop();
                    showingRange = false;
                    Fire();
                }
            }
        }
        else if (Input.GetKey(KeyCode.R)) // NOTE: Remove this else if when power manager is done
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
