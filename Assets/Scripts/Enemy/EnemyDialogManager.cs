using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyDialogManager : MonoBehaviour
{
    [SerializeField] private float timeRandomDialog = 5f;
    [SerializeField] private float timeDurationDialog = 4.5f;
    [SerializeField] private int probablityDialog = 2;
    [SerializeField] private float clockDialog = 0f;
    private TMPDialogue dialogs;

    private void Start()
    {
        dialogs = GetComponent<TMPDialogue>();
    }

    private void update()
    {
        
    }
}
