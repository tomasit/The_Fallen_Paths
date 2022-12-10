using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutoDeathManager : MonoBehaviour
{
    [SerializeField] private TransitionScreen _transitionScreen;
    [SerializeField] private GameObject _player;
    private Coroutine _deathCoroutine = null;
    private Vector3 _lastCheckpoint;

    public void SetCheckpoint(Vector3 position)
    {
        _lastCheckpoint = position;
    }

    public bool IsInTransition()
    {
        return _transitionScreen.GetTransitionState() != TransitionScreen.TransitionState.NONE;
    }

    public TransitionScreen.TransitionState GetTransitionState()
    {
        return _transitionScreen.GetTransitionState();
    }

    private IEnumerator Death()
    {
        _transitionScreen.StartDeadTransition();
        _player.GetComponent<PlayerController>().BlockInput(true);
        _player.GetComponent<ComputeInteraction>().BlockInput(true);
        _player.GetComponent<PowerManager>().canUseAnyPower = false;
        while (_transitionScreen.GetTransitionState() != TransitionScreen.TransitionState.MIDDLE)
            yield return null;
        _player.GetComponent<PowerManager>().ResetEverything();
        _player.GetComponent<PlayerHealthWrapper>().Revive();
        _player.GetComponent<HideInteraction>().Cancel();
        _player.transform.position = _lastCheckpoint;
        while (_transitionScreen.GetTransitionState() != TransitionScreen.TransitionState.NONE)
            yield return null;
        _player.GetComponent<PlayerController>().BlockInput(false);
        _player.GetComponent<ComputeInteraction>().BlockInput(false);
        _player.GetComponent<PowerManager>().canUseAnyPower = true;
    }

    public void TriggerDeath()
    {
        _deathCoroutine = StartCoroutine(Death());
    }
}
    
