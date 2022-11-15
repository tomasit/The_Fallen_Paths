using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeathManager : MonoBehaviour
{
    [SerializeField] private TransitionScreen _transitionScreen;
    [SerializeField] private GameObject _player;
    private Coroutine _deathCoroutine = null;

    private IEnumerator Death()
    {
        _transitionScreen.StartDeadTransition();
        _player.GetComponent<PlayerController>().BlockInput(true);
        _player.GetComponent<TestComputeInteraction>().BlockInput(true);
        _player.GetComponent<PowerManager>().canUseAnyPower = true;
        while (_transitionScreen.GetTransitionState() != TransitionScreen.TransitionState.MIDDLE)
            yield return null;
        _player.GetComponent<PlayerHealthWrapper>().Revive();
        _player.GetComponent<HideInteraction>().Cancel();
        _player.transform.position = SaveManager.DataInstance.GetValue();
    }

    public void TriggerDeath()
    {
        _deathCoroutine = StartCoroutine(Death());
    }
}
