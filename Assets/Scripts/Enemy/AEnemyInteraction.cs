using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AEnemyInteraction : MonoBehaviour
{
    public LayerMask[] layersToIgnore;

    public ActionType action;

    public float coolDown;
    public float clockCoolDown = 0f;
    public bool isAtdistanceToInteract = false;

    public float damage;


    public void IgnoreLayers()
    {
        foreach (LayerMask layer in layersToIgnore)
        {
            // NOTE : if the layerMask is the 31, it didn't work
            int layerValue = (int)Mathf.Log(layer.value, 2);

            Physics2D.IgnoreLayerCollision(layerValue, gameObject.layer, true);
        }
    }

    public abstract void Interact(GameObject obj, ActionType action);
}
