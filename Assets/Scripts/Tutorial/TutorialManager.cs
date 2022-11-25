using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private EnemyEventsManager _enemyEvent;
    [SerializeField] private EnemyDetectionManager _enemy2;
    [SerializeField] private EnemyDetectionManager _enemy3;
    [SerializeField] private PowerManager _powerManager;
    [SerializeField] private Transform _respawnRoom2;
    [SerializeField] private TutoDeathManager _deathManager;
    [SerializeField] private TMPDialogue _dialogue;
    [SerializeField] private TransitionScreen _transitionDialogue;
    [SerializeField] private EnemyDetectionManager _enemy1;
    [SerializeField] private SubLevelChange _door1;
    [SerializeField] private Transform _respawnRoom1;
    [SerializeField] private Transform _respawnEnemy1Pos;
    [SerializeField] private SubLevelChange _door2;
    [SerializeField] private Transform _canvasTransform;
    [SerializeField] private GameObject _nameBox;
    [SerializeField] private PlayerController _controller;
    [SerializeField] private InteractionSelectorEnable _firstDoorEnabler;
    [SerializeField] private InteractionSelectorEnable _secondDoorEnabler;
    [SerializeField] private DestroyInteraction _cageInteraction;
    [SerializeField] private TestComputeInteraction _interactor;
    [SerializeField] private float _fadeDuration;
    [SerializeField] private RemoteObjectControl _power; // replace by power manager
    private Coroutine _tutorialCoroutine;
    private bool _startTutorial;
    private GameObject _nameBoxRef;

    private IEnumerator ReminderOffset(string dialogueName)
    {
        float timer = 0.0f;
        float remindOffset = 5.0f;
        while (true)
        {
            if (_dialogue.IsFinish())
            {
                if (timer >= remindOffset)
                {
                    timer = 0.0f;
                    _dialogue.StartDialogue(dialogueName);
                }
                else
                    timer += Time.deltaTime;
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    private IEnumerator WaitForJump()
    {
        Coroutine reminder = StartCoroutine(ReminderOffset("JumpReminder"));
        while (!Input.GetButtonDown("Jump"))
        {
            yield return null;
        }
        StopCoroutine(reminder);
    }

    private IEnumerator WaitForInput()
    {
        float timer = 0.0f;
        float leftTimer = 0.0f;
        float rightTimer = 0.0f;
        for (bool left = false, right = false; !left || !right;)
        {
            if (_dialogue.IsFinish())
            {
                timer += Time.deltaTime;
                if (timer >= 5.0f)
                {
                    _dialogue.StartDialogue("KeyReminder");
                    timer = 0.0f;
                }
            }

            float input = Input.GetAxisRaw("Horizontal");
            if (input < -0.01f)
            {
                leftTimer += Time.deltaTime;
                if (leftTimer >= 0.5f)
                    left = true;
            }
            if (input > 0.01f)
            {
                rightTimer += Time.deltaTime;
                if (rightTimer >= 0.5f)
                    right = true;
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    private IEnumerator WaitForDestroy()
    {
        Coroutine reminder = StartCoroutine(ReminderOffset("InteractionReminder"));
        while (!_cageInteraction.IsObjectDestroy())
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                StopCoroutine(reminder);
                reminder = StartCoroutine(ReminderOffset("InteractionReminder"));
            }
            yield return null;
        }
        StopCoroutine(reminder);
    }

    private IEnumerator WaitForDialogueToFinish()
    {
        while (!_dialogue.IsFinish())
            yield return null;
    }

    private IEnumerator WaitForPower()
    {
        bool condition = false;
        Coroutine reminder = StartCoroutine(ReminderOffset("ActivePowerReminder"));
        while (!condition)
        {
            condition = _power.HasObjectInRange() & _controller.isGrounded();

            if (!_power.activated)
            {
                if (reminder == null)
                {
                    reminder = StartCoroutine(ReminderOffset("ActivePowerReminder"));
                }
            }
            else
            {
                if (reminder != null)
                    StopCoroutine(reminder);
                reminder = null;
            }
            yield return null;
        }
        if (reminder != null)
            StopCoroutine(reminder);
    }

    private IEnumerator WaitForClick()
    {
        Coroutine reminder = StartCoroutine(ReminderOffset("ClickReminder"));
        while (_power.activated)
        {
            yield return null;
        }
        StopCoroutine(reminder);
    }

    private IEnumerator WaitForRespawn()
    {
        while (_deathManager.GetTransitionState() != TransitionScreen.TransitionState.MIDDLE)
            yield return null;
        _controller.BlockInput(true);
        _enemy1.transform.position = _respawnEnemy1Pos.position;
        _enemy1.SetState(DetectionState.Freeze);
        while (_deathManager.GetTransitionState() != TransitionScreen.TransitionState.NONE)
            yield return null;
       _controller.BlockInput(true);
        _dialogue.StartDialogue("TryAgain");
        yield return StartCoroutine(WaitForDialogueToFinish());
        _controller.BlockInput(false);
       _enemy1.SetState(DetectionState.None);
    }

    private IEnumerator WaitForRespawnRoom2()
    {
        while (_deathManager.GetTransitionState() != TransitionScreen.TransitionState.MIDDLE)
            yield return null;
        _controller.BlockInput(true);
        _enemy2.transform.position = _enemyEvent.Enemies[0].roomProprieties.targets[0].position;
        _enemy2.SetState(DetectionState.None);
        _enemy3.transform.position = _enemyEvent.Enemies[1].roomProprieties.targets[0].position;
        _enemy3.SetState(DetectionState.None);
        while (_deathManager.GetTransitionState() != TransitionScreen.TransitionState.NONE)
            yield return null;
        _controller.BlockInput(false);
    }

    private IEnumerator GoToNextRoom()
    {
        while (!_door2.IsInTransition())
        {
            if (_deathManager.GetTransitionState() != TransitionScreen.TransitionState.NONE)
            {
                yield return StartCoroutine(WaitForRespawn());
            }
            else
                yield return null;
        }
    }

    private IEnumerator EndOfTutorial()
    {
        while (true)
        {
            if (_deathManager.GetTransitionState() != TransitionScreen.TransitionState.NONE)
            {
                yield return StartCoroutine(WaitForRespawnRoom2());
            }
            else
                yield return null;            
        }
    }

    private IEnumerator TutorialCoroutine()
    {
        _interactor.BlockInput(true);
        _controller.BlockInput(true);
        _transitionDialogue.StartBeginTransition("Room_1_description");
        while (_transitionDialogue.GetTransitionState() != TransitionScreen.TransitionState.NONE)
            yield return null;
        if (SaveManager.DataInstance.GetPlayerInfo()._playerName == "")
            _nameBoxRef = Instantiate(_nameBox, _canvasTransform);
        while (SaveManager.DataInstance.GetPlayerInfo()._playerName == "")
            yield return null;
        if (_nameBoxRef != null)
            Destroy(_nameBoxRef);
        _dialogue.StartDialogue("Welcome");
        yield return StartCoroutine(WaitForDialogueToFinish());
        _controller.BlockInput(false);
        _interactor.BlockInput(false);
        yield return StartCoroutine(WaitForDestroy());
        _controller.BlockInput(true);
        _dialogue.StartDialogue("KeyInstruction");
        yield return StartCoroutine(WaitForDialogueToFinish());
        _controller.BlockInput(false);
        yield return StartCoroutine(WaitForInput());
        _controller.BlockInput(true);
        _dialogue.StartDialogue("JumpInstruction");
        yield return StartCoroutine(WaitForDialogueToFinish());
        _controller.BlockInput(false);
        yield return StartCoroutine(WaitForJump());
        _firstDoorEnabler.Interact();
        _dialogue.StartDialogue("GoToPowerLevel");
        while (!_door1.IsInTransition())
            yield return null;
        if (!_dialogue.IsFinish())
            _dialogue.StopDialogue();
        while (_door1.IsInTransition())
            yield return null;
        _deathManager.SetCheckpoint(_respawnRoom1.position);
        _dialogue.StartDialogue("PowerExplanation");
        _controller.BlockInput(true);
        _interactor.BlockInput(true);
        yield return StartCoroutine(WaitForDialogueToFinish());
        yield return new WaitForSeconds(2); // replace by unlock power animation
        _powerManager._powers[0].unlocked = true;
        _dialogue.StartDialogue("PowerInstruction");
        yield return StartCoroutine(WaitForDialogueToFinish());
        _controller.BlockInput(false);
        _interactor.BlockInput(false);
        yield return StartCoroutine(WaitForPower());
        _controller.BlockInput(true);
        _dialogue.StartDialogue("ClickToInteract");
        yield return StartCoroutine(WaitForDialogueToFinish());
        yield return StartCoroutine(WaitForClick());
        _dialogue.StartDialogue("CongratPower");
        yield return StartCoroutine(WaitForDialogueToFinish());
        _enemy1.detectionState = DetectionState.None;
        _controller.BlockInput(false);
        _secondDoorEnabler.Interact();
        yield return StartCoroutine(GoToNextRoom());
        _deathManager.SetCheckpoint(_respawnRoom2.position);
        yield return StartCoroutine(EndOfTutorial());
    }

    private void Update()
    {
        if (!_startTutorial)
        {
            _startTutorial = true;
            StartCoroutine(TutorialCoroutine());
        }
    }

    private void Start()
    {
        _controller.transform.gameObject.GetComponent<BasicHealthWrapper>().SetMaxHealth(1);
    }
}
