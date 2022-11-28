using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class APower : MonoBehaviour
{
    public bool firingPower = false;
    protected PowerManager _powerManager = null;
    protected virtual void Start()
    {
        _powerManager = FindObjectOfType<PowerManager>();
    }

    public abstract void Use();
    public virtual void Fire()
    {
        _powerManager.ActivatePowerCooldownFromStackTrace(2);
    }
    public abstract void Cancel();
}
