using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private SubLevelChange _door1;
    [SerializeField] private Transform _canvasTransform;
    [SerializeField] private GameObject _nameBox;
    [SerializeField] private PlayerController _controller;
    [SerializeField] private InteractionSelectorEnable _firstDoorEnabler;
    [SerializeField] private DestroyInteraction _cageInteraction;
    [SerializeField] private TestComputeInteraction _interactor;
    [SerializeField] private string _description;
    [SerializeField] private Image _fadeImage;
    [SerializeField] private TextMeshProUGUI _tmproUGUI;
    [SerializeField] private float _fadeDuration;
    private TMPDialogue _dialogue;
    private Coroutine _tutorialCoroutine;
    private bool _startTutorial;
    private GameObject _nameBoxRef;

    private IEnumerator BlackFade(bool fadeIn)
    {
        Color baseImgColor = _fadeImage.color;
        Color nextImgColor = new Color(baseImgColor.r, baseImgColor.g, baseImgColor.b, fadeIn ? 1.0f : 0.0f);
        Color baseTxtColor = _tmproUGUI.color;
        Color nextTxtColor = new Color(baseTxtColor.r, baseTxtColor.g, baseTxtColor.b, fadeIn ? 1.0f : 0.0f);

        _tmproUGUI.text = _description;

        yield return (new WaitForSeconds(3.0f));

        for (float t = 0; t < 1.0f; t += Time.deltaTime / _fadeDuration)
        {
            _fadeImage.color = Color.Lerp(baseImgColor, nextImgColor, t);
            _tmproUGUI.color = Color.Lerp(baseTxtColor, nextTxtColor, t);
            yield return null;
        }
    }

    private IEnumerator WaitForJump()
    {
        float timer = 0.0f;
        while (!Input.GetButtonDown("Jump"))
        {
            if (_dialogue.IsFinish())
            {
                if (timer >= 5.0f)
                {
                    timer = 0.0f;
                    _dialogue.StartDialogue("JumpReminder");
                }
                else
                    timer += Time.deltaTime;
            }

            yield return new WaitForSeconds(Time.deltaTime);
        }
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
        float time = 0.0f;
        while (!_cageInteraction.IsObjectDestroy())
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                time = 0.0f;
            }
            if (_dialogue.IsFinish())
            {
                if (time >= 5.0f)
                {
                    time = 0.0f;
                    _dialogue.StartDialogue("InteractionReminder");
                }
                else
                    time += Time.deltaTime;
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    private IEnumerator WaitForDialogueToFinish()
    {
        while (!_dialogue.IsFinish())
            yield return null;
    }

    // private IEnumerator WaitForPower()
    // {
    //     yield 
    // }

    private IEnumerator TutorialCoroutine()
    {
        _interactor.BlockInput(true);
        _controller.BlockInput(true);
        yield return StartCoroutine(BlackFade(false));
        if (SaveManager.DataInstance.GetPlayerInfo()._playerName == "")
            _nameBoxRef = Instantiate(_nameBox, _canvasTransform);
        while (SaveManager.DataInstance.GetPlayerInfo()._playerName == "")
            yield return null;
        if (_nameBoxRef != null)
            Destroy(_nameBoxRef);
        _dialogue.StartDialogue("Welcome");
        yield return WaitForDialogueToFinish();
        _controller.BlockInput(false);
        _interactor.BlockInput(false);
        yield return WaitForDestroy();
        _controller.BlockInput(true);
        _dialogue.StartDialogue("KeyInstruction");
        yield return WaitForDialogueToFinish();
        _controller.BlockInput(false);
        yield return WaitForInput();
        _controller.BlockInput(true);
        _dialogue.StartDialogue("JumpInstruction");
        yield return WaitForDialogueToFinish();
        _controller.BlockInput(false);
        yield return WaitForJump();
        _firstDoorEnabler.Interact();
        _dialogue.StartDialogue("GoToPowerLevel");
        while (!_door1.IsInTransition())
            yield return null;
        if (!_dialogue.IsFinish())
            _dialogue.StopDialogue();
        while (_door1.IsInTransition())
            yield return null;
        _dialogue.StartDialogue("PowerExplanation");
        _controller.BlockInput(true);
        _interactor.BlockInput(true);
        yield return WaitForDialogueToFinish();
        yield return new WaitForSeconds(2); // replace by unlock power animation
        _dialogue.StartDialogue("PowerInstruction");
        yield return WaitForDialogueToFinish();
        _controller.BlockInput(false);
        _interactor.BlockInput(false);
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
        _fadeImage.color = new Color(_fadeImage.color.r, _fadeImage.color.g, _fadeImage.color.b, 1.0f);
        _tmproUGUI.color = new Color(_tmproUGUI.color.r, _tmproUGUI.color.g, _tmproUGUI.color.b, 1.0f);
        _dialogue = GetComponent<TMPDialogue>();
        if (_description.Contains("\\n"))
        {
            _description = _description.Replace("\\n", "\n");
        }
    }
}
