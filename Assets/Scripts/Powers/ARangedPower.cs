using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ARangedPower -> Preview(), CanCastPower(), 

// AVisualCircleRangedPower
public abstract class ARangedPower : APower
{
    public float rangeRadius = 0;
    public bool activated = false;

    // Update is called once per frame
    protected bool mouseDistranceIsCorrect()
    {
        Vector2 distance = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        return distance.magnitude <= rangeRadius;
    }

    public override void Use()
    {
        activated = true;
    }

    protected abstract void Preview();
    protected abstract void UnPreview();

    protected abstract bool canCastPower();
}
