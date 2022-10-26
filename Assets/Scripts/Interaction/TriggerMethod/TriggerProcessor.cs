using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerProcessor : MonoBehaviour
{
    protected bool _isDisable = false;

    public void DisableTrigger()
    {
        _isDisable = true;
    }

    public virtual void Trigger()
    {
        InteractionProcessor processor = GetComponent<InteractionProcessor>();
        if (processor != null)
            processor.Interact();
    }
}