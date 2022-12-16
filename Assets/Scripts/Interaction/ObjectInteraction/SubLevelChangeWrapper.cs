using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubLevelChangeWrapper : AInteractable
{
    [SerializeField] private SubLevelChange [] subLevelInteractions;
    [SerializeField] private int indexLevel;

    void Start()
    {
    }

    void Update()
    {

    }

    public override void Interact()
    {
        if (subLevelInteractions.Length != 2) {
            if (subLevelInteractions[0] == null || subLevelInteractions[1] == null) {
                return;
            }
        }
        if (!subLevelInteractions[0].IsInTransition() && !subLevelInteractions[1].IsInTransition()) {
            //on lance un interact
            subLevelInteractions[indexLevel].Interact();
            //on change l'index
            indexLevel = (indexLevel == 0) ? 1 : 0;
        }
    }

    public override void Save()
    {
    }

    public override void Load()
    {
    }

}