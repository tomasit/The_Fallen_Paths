using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ARangedPower : APower
{
    public float rangeRadius = 0;
    public bool activated = false;

    protected bool mouseDistranceIsCorrect()
    {
        Vector2 distance = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        return distance.magnitude <= rangeRadius;
    }

    public override void Use()
    {
        activated = true;
    }

    public virtual void CancelRange()
    {
        activated = false;
    }

    public override void Fire()
    {
        _powerManager.ActivatePowerCooldownFromStackTrace(2);
    }

    protected abstract void Preview();
    protected abstract void UnPreview();
    protected abstract bool canCastPower();
}
