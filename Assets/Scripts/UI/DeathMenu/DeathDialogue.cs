using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeathDialogue : MonoBehaviour
{
    [SerializeField] private TMPDialogue _dialogue;

    private void Start()
    {
        _dialogue.SetCanvasTransform(FindObjectOfType<Canvas>().transform);
        _dialogue.StartDialogue("DeadDialogue");
    }
}
    