using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialRatManager : MonoBehaviour
{
    [SerializeField] private TMPDialogue _dialogue;
    [SerializeField] private Transform _ratHole;
    [SerializeField] private GlowOnTouch _glowOnTuchCup;
    [SerializeField] private AddPowerInteraction _addPowerInteraction;
    
    private PowerMenuManager _powerUiManager;
    private PowerManager _powerManager;
    private ComputeInteraction _interactor;
    private PlayerController _controller;
    private Rigidbody2D _rigidBody;

    [SerializeField] private bool _startTutorial;
    [SerializeField] private bool _isDisable;

    private void Start()
    {
        _powerUiManager = (PowerMenuManager)FindObjectOfType<PowerMenuManager>();
        _controller = (PlayerController)FindObjectOfType<PlayerController>();
        _powerManager = (PowerManager)FindObjectOfType<PowerManager>();
        _interactor = (ComputeInteraction)FindObjectOfType<ComputeInteraction>();
        _rigidBody = (Rigidbody2D)_controller.gameObject.GetComponent<Rigidbody2D>();
    }

    private IEnumerator TutorialCoroutine()
    {
        _glowOnTuchCup.Trigger(true);
        _controller.BlockInput(true);
        _dialogue.StartDialogue("PotionCatch");
        yield return StartCoroutine(WaitForDialogueToFinish());
        yield return StartCoroutine(WaitForKeyPressed(KeyCode.E));
        _interactor.BlockInput(true);
        _dialogue.StartDialogue("GetRatPower");
        yield return StartCoroutine(WaitForDialogueToFinish());
        _addPowerInteraction.Interact();
        _powerUiManager.AblePowerMenu();
        _powerUiManager.BlockUntilPowerAssign(1);
        while (_powerUiManager.WaitForAssignIndex())
            yield return null;
        _dialogue.StartDialogue("PowerInstruction1");
        yield return StartCoroutine(WaitForDialogueToFinish());
        _glowOnTuchCup.Trigger(false);
        _rigidBody.velocity = Vector2.zero;
        _controller.Move(0);
        yield return StartCoroutine(WaitForMovingToRatHole());
        _dialogue.StartDialogue("PowerInstruction2");
        yield return StartCoroutine(WaitForDialogueToFinish());
        yield return StartCoroutine(WaitForKeyPressed(FindPowerKey(1)));
        _dialogue.StartDialogue("EndingTutorial");
        yield return StartCoroutine(WaitForDialogueToFinish());
        _interactor.BlockInput(false);
        _controller.BlockInput(false);
    }

    private KeyCode FindPowerKey(int powerIndex)
    {
        for (int idx = 0; idx < _powerManager._powers.Count; ++idx) {
            if (idx == powerIndex) {
                return _powerManager._powers[idx].key;
            }
        }

        return KeyCode.None;
    }

    private IEnumerator WaitForKeyPressed(KeyCode key)
    {
        while (!Input.GetKeyDown(key)) {
            yield return null;
        }
    }

    private IEnumerator WaitForMovingToRatHole()
    {
        while (_controller.transform.position.x > _ratHole.position.x) {
            _controller.Move(-1);
            _controller.AnimateMovement(false);
            yield return null;
        }
        _controller.BlockInput(false);
        _controller.AnimateMovement(true);
        _controller.Move(0);
        _controller.BlockInput(true);
    }

    private IEnumerator WaitForDialogueToFinish()
    {
        while (!_dialogue.IsFinish())
            yield return null;
    }
    

    private void Update()
    {
        if (_startTutorial)
        {
            _startTutorial = false;
            _isDisable = true;
            StartCoroutine(TutorialCoroutine());
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!_isDisable)
        {
            if (LayerMask.NameToLayer("Player") == collider.gameObject.layer)
            {
                _startTutorial = true;
            }
        }
    }
}