using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class APower : MonoBehaviour
{
    protected bool firingPower = false;
    public IEnumerator GetEnumerator() { return (IEnumerator)this; }
    public abstract void Use();
    public abstract void Fire();
}
