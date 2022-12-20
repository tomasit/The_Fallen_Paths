using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class RespawnManager : MonoBehaviour
{
    private Transform _player;
    private TransitionScreen _screenTransition;
    private bool _isDead = false;

    private void Start()
    {
        _screenTransition = FindObjectOfType<TransitionScreen>();
        _player = FindObjectOfType<PlayerController>().transform;
    }

    private IEnumerator DeathCoroutine()
    {
        yield return new WaitForSeconds(.8f);
        _screenTransition.StartBeginTransition("Death");
        while (_screenTransition.GetTransitionState() != TransitionScreen.TransitionState.END)
            yield return null;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void Update()
    {
        if (!_isDead && _player.GetComponent<Health>().isDead())
        {
            _isDead = true;
            StartCoroutine(DeathCoroutine());
        }
    }
}
    