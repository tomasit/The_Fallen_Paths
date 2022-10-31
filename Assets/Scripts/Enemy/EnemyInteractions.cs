using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static EnemyInfo;

//laders, stairs ...
public class EnemyInteractions : AEnemyInteraction
{

    void Start()
    {
        IgnoreLayers();
    }

    void Update()
    {
    }

    public override void Interact(GameObject obj, ActionType action)
    {
    }
}
