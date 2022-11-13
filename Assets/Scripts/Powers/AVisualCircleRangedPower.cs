using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ARangedPower -> Preview(), CanCastPower(), 

// AVisualCircleRangedPower
public abstract class AVisualCircleRangedPower : ARangedPower
{
    [SerializeField] private ParticleSystem _rangePrefab = null;
    private ParticleSystem _rangeObject = null;

    [SerializeField] protected ParticleSystem.MinMaxGradient _rangeColor = Color.white;
    // Update is called once per frame

    public override void Use()
    {
        activated = true;
        showRange();
    }

    protected void showRange()
    {
        _rangeObject = Instantiate(_rangePrefab, transform);

        _rangeObject.transform.localScale = new Vector3(1, 1, 1);

        var rangeShape = _rangeObject.shape;
        rangeShape.radius = rangeRadius;

        var rangeMain = _rangeObject.main;
        rangeMain.startColor = _rangeColor;

        _rangeObject.Play();
    }

    void StopAndDetachRangeObject()
    {
        _rangeObject.Stop();
        var oldScale = _rangeObject.transform.localScale;
        _rangeObject.transform.parent = transform.parent;
        _rangeObject.transform.localScale = oldScale;
    }

    public override void CancelRange()
    {
        base.CancelRange();
        StopAndDetachRangeObject();
        UnPreview();
    }

    void Update()
    {
        if (activated)
        {
            if (mouseDistranceIsCorrect())
            {
                if (canCastPower() && Input.GetMouseButtonDown(0))
                {
                    CancelRange();
                    firingPower = true;
                }
                else
                    Preview();
            }
            else
                UnPreview();
        }
        else if (_rangeObject != null && _rangeObject.particleCount == 0)
        {
            Destroy(_rangeObject.gameObject);
            _rangeObject = null;
        }
        else if (firingPower)
            Fire();

    }
}
