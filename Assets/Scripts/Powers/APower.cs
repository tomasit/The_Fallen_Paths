using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class APower : MonoBehaviour
{
    public bool firingPower = false;

    public abstract void Use();
    public abstract void Fire();
    public abstract void Cancel();
}
