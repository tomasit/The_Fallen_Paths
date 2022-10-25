using UnityEngine;

public abstract class AView : MonoBehaviour {
    public virtual void Hide() => gameObject.SetActive(false);
    public virtual void Show() => gameObject.SetActive(true);
}