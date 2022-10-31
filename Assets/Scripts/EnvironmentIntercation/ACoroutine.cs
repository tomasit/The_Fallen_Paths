using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PersistentId))]
public abstract class ACoroutine : MonoBehaviour
{
    public enum DescriptionHeight : int
    {
        NONE = 0,
        TWO_STATE_DESCRIPTION = 1,
        LOCK = 2
    }
    protected DescriptionHeight _descriptionHeight = DescriptionHeight.NONE;

    public DescriptionHeight GetDescriptionHeight() { return _descriptionHeight; }
    public abstract IEnumerator Interact(Transform obj = null);
    //public abstract void Save();
    //public abstract void Load();
    public virtual string GetDescription() { return ""; }
}
