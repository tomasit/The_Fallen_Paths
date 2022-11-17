using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class EndOfTutorialInteraction : AInteractable
{
    [SerializeField] private TransitionScreen _transition;

    private IEnumerator BackToBegin()
    {
        _transition.StartBeginTransition("EndOfTutorial");
        while (_transition.GetTransitionState() != TransitionScreen.TransitionState.NONE)
            yield return null;
        SaveManager.DataInstance.GetPlayerInfo()._playerName = "";
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public override void Interact()
    {
        StartCoroutine(BackToBegin());
    }

    public override void Save()
    {}
    
    public override void Load()
    {}
}
