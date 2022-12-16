using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public struct PowerIUSlot
{
    public int powerIdx;
    public GameObject entity;
    public Transform initialPos;
    public TMP_Text powerName;
    public string powerDescriptionName;
    public GameObject ImagePowerUnLocked;
    public GameObject ImagePowerLocked;
    public GameObject ImagePowerHightLight;
    public bool unlocked;
    //APower maybe
}

public class PowerMenuManager : MonoBehaviour
{   
    [Header("Basics")]
    [SerializeField] private bool _isDisplay;
    [SerializeField] private GameObject _powerUi;
    [SerializeField] private GameObject _powerGui;
    [SerializeField] private float _moveFrameDist = 1f;
    [SerializeField] private float _moveFrameTime = 0.025f;
    [SerializeField] private float _moveFramePrecision = 1f;
    [SerializeField] private Transform _guiMenuPos;
    [SerializeField] private Transform _uiMenuPos;
    [SerializeField] private Transform _guiGamePos;
    [SerializeField] private Transform _uiBasePos;

    [Header("Powers")]
    [SerializeField] public PowerIUSlot [] _uiPowers;
    [SerializeField] private PowerIUSlot _currentPower;

#region UI_OBJS
    [Header("ChooseAction")]
    [SerializeField] private bool _isDisplayingChoose;
    [SerializeField] private GameObject _descriptionButton;
    [SerializeField] private GameObject _assignButton;
    [Header("KeyChoose")]
    [SerializeField] private bool _isDisplayingKeyChoosing;
    [SerializeField] private GameObject _instructionKeyText;
    [SerializeField] private TMP_Text _KeyInputText;
    [SerializeField] private KeyCode _currentKey;
    [SerializeField] private float _clockTick;
    [SerializeField] private float _tick = 0.5f;
    [SerializeField] private string _keyColor = "yellow";
    [SerializeField] private string _currentColor;
    [SerializeField] private GameObject _AssignSubmitButton;
    [Header("SlotChoose")]
    [SerializeField] private bool _isDisplayingSlotChoosing;
    [SerializeField] private GameObject _instructionSlotText;
    [SerializeField] private GameObject _slotPanel;
    [SerializeField] private GameObject _prefabSlotButton;
    [Header("Desciptions")]
    [SerializeField] private bool _isDisplayingDescription;
    [SerializeField] private TMPDialogue _descriptionDialogStory;
    [SerializeField] private TMPDialogue _descriptionDialogUsage;
    [SerializeField] private Transform _posSlotPowerDescription;
#endregion

    [Header("Description Animation")]
    [SerializeField] private Transform _posSlotDescriptionStoryBase;
    [SerializeField] private Transform _posSlotDescriptionStoryFollow;
    [SerializeField] private Transform _posSlotDescriptionStory;
    [SerializeField] private Transform _posSlotDescriptionUsageBase;
    [SerializeField] private Transform _posSlotDescriptionUsageFollow;
    [SerializeField] private Transform _posSlotDescriptionUsage;
    private Coroutine _moveGuiCoroutine;
    private Coroutine _moveUiCoroutine;
    private Coroutine _moveIconDescription;
    private Coroutine _movePannelDescriptionStory;
    private Coroutine _movePannelDescriptionUsage;

    private PowerManager _powerManager;
    private PowerGUIManager _powerGUiManger;
    private PauseManager _pauseManager;
    private Canvas _canvas;

    private void Start()
    {
        _pauseManager = (PauseManager)FindObjectOfType(typeof(PauseManager));
        _powerGUiManger = (PowerGUIManager)FindObjectOfType(typeof(PowerGUIManager));
        _powerManager = (PowerManager)FindObjectOfType(typeof(PowerManager));
        _canvas = (Canvas)FindObjectOfType(typeof(Canvas));
        _currentColor = _keyColor;
    }

    private bool isOnBaseMenu()
    {
        return !_isDisplayingChoose && !_isDisplayingDescription && !_isDisplayingChoose 
                && !_isDisplayingKeyChoosing && !_isDisplayingSlotChoosing;
    }

    private void Update()
    {
        if (_isDisplay) {
            _powerUi.SetActive(true);
            if (Input.GetKeyDown(KeyCode.Escape) && isOnBaseMenu()) {
                if (_isDisplayingDescription) {
                    UnAbleDescription();
                }
                UnAblePowerMenu();
            }
        } else {
            return;
        }

        if (_isDisplay && isOnBaseMenu()) {
            ChoosePower();
        }

        if (_isDisplayingKeyChoosing) {
            InputKeyInText();
            TickKeyInput();
        }

        else if (Input.GetKeyDown(KeyCode.M) && _isDisplay) {
            if (_isDisplayingDescription) {
                UnAbleDescription();
            }
            UnAblePowerMenu();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && _isDisplayingChoose) {
            UnAbleChoose();
            UnCenterPower();
        }
        if (Input.GetKeyDown(KeyCode.Escape) && _isDisplayingDescription) {
            UnAbleDescription();
            AbleChoose();
        }
        if (Input.GetKeyDown(KeyCode.Escape) && _isDisplayingKeyChoosing) {
            UnAbleChooseKey();
            AbleChoose();
        }
        if (Input.GetKeyDown(KeyCode.Escape) && _isDisplayingSlotChoosing) {
            UnAbleChooseSlot();
            AbleChooseKey();
        }
    }

#region ACTIVE_UI
    public void AbleChooseSlot()
    {
        _isDisplayingSlotChoosing = true;
        UnAbleChooseKey();

        _instructionSlotText.SetActive(true);
        _slotPanel.SetActive(true);
        LoadSlotsUI();
    }
    public void UnAbleChooseSlot()
    {
        _isDisplayingSlotChoosing = false;
        UnLoadSlotsUI();

        _instructionSlotText.SetActive(false);
        _slotPanel.SetActive(false);
    }
    public void AbleChooseKey()
    {
        _isDisplayingKeyChoosing = true;
        UnAbleChoose();

        _instructionKeyText.SetActive(true);
        _KeyInputText.gameObject.SetActive(true);
        _AssignSubmitButton.SetActive(true);
        _currentKey = _powerManager._powers[_currentPower.powerIdx].key;
    }
    public void UnAbleChooseKey()
    {
        _isDisplayingKeyChoosing = false;

        _instructionKeyText.SetActive(false);
        _KeyInputText.gameObject.SetActive(false);
        _AssignSubmitButton.SetActive(false);
    }
    public void AbleDescription()
    {
        _isDisplayingDescription = true;
        UnAbleChoose();

        _descriptionDialogStory.StartDialogue(_currentPower.powerName.text);
        _descriptionDialogUsage.StartDialogue(_currentPower.powerName.text);
        if (_movePannelDescriptionStory != null)
            StopCoroutine(_movePannelDescriptionStory);
        _movePannelDescriptionStory = StartCoroutine(MoveGUI(_posSlotDescriptionStory.position, _posSlotDescriptionStoryFollow.gameObject.transform));
        if (_movePannelDescriptionUsage != null)
            StopCoroutine(_movePannelDescriptionUsage);
        _movePannelDescriptionUsage = StartCoroutine(MoveGUI(_posSlotDescriptionUsage.position, _posSlotDescriptionUsageFollow.gameObject.transform));
    }
    public void UnAbleDescription()
    {
        _isDisplayingDescription = false;

        if (_movePannelDescriptionStory != null)
            StopCoroutine(_movePannelDescriptionStory);
        StartCoroutine(MoveAndDisableDialog(_posSlotDescriptionStoryBase.position, _posSlotDescriptionStoryFollow.gameObject.transform));
        if (_movePannelDescriptionUsage != null)
            StopCoroutine(_movePannelDescriptionUsage);
        StartCoroutine(MoveAndDisableDialog(_posSlotDescriptionUsageBase.position, _posSlotDescriptionStoryFollow.gameObject.transform));
    }

    private void AbleChoose()
    {
        _isDisplayingChoose = true;
        
        //! fade
        _descriptionButton.SetActive(true);
        _assignButton.SetActive(true);
    }
    private void UnAbleChoose()
    {
        _isDisplayingChoose = false;
        
        _descriptionButton.SetActive(false);
        _assignButton.SetActive(false);
    }

    private void CenterPower()
    {
        if (_moveIconDescription != null)
            StopCoroutine(_moveIconDescription);
        _moveIconDescription = StartCoroutine(MoveGUI(_posSlotPowerDescription.position, _currentPower.entity.transform));
        // fade la color a la mano
        foreach (var power in _uiPowers) {
            if (_currentPower.powerName.text != power.powerName.text) {
                power.entity.SetActive(false);
            }
        }
    }
    private void UnCenterPower()
    {
        if (_moveIconDescription != null)
            StopCoroutine(_moveIconDescription);
        _moveIconDescription = StartCoroutine(MoveGUI(_currentPower.initialPos.position, _currentPower.entity.transform));
        // fade la color a la mano
        foreach (var power in _uiPowers) {
            if (_currentPower.powerName.text != power.powerName.text) {
                power.entity.SetActive(true);
            }
        }
        //! fade
    }
    public void AblePowerMenu()
    {
        _pauseManager.Enable(false);
        Time.timeScale = 0f;
        _isDisplay = true;
        LoadPowerIUSlots();
        if (_moveGuiCoroutine != null)
            StopCoroutine(_moveGuiCoroutine);
        _moveGuiCoroutine = StartCoroutine(MoveGUI(_guiMenuPos.position, _powerGui.transform));

        if (_moveUiCoroutine != null)
            StopCoroutine(_moveUiCoroutine);
        _moveUiCoroutine = StartCoroutine(MoveGUI(_uiMenuPos.position, _powerUi.transform));
    }
    public void UnAblePowerMenu()
    {
        _isDisplay = false;
        Time.timeScale = 1f;
        _pauseManager.Enable(true);
        if (_moveGuiCoroutine != null)
            StopCoroutine(_moveGuiCoroutine);
        StartCoroutine(MoveGUI(_guiGamePos.position, _powerGui.transform));
        StartCoroutine(MoveAndSetActiveFalseMenu(_uiBasePos.position, _powerUi.transform));
        /*if (_isDisplayingChoose) {
            UnAbleChoose();
        }*/
    }
#endregion

    private void InputKeyInText()
    {
        foreach(KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(kcode)) {
                if (!IsIllegalKey(kcode)) {
                    _currentKey = kcode;
                }
            }
        }
    }

    //l'icone est au bonne endroit pas la lettre parfois

    private bool IsIllegalKey(KeyCode key)
    {
        return (
            key == KeyCode.E || key == KeyCode.Space
            || key == KeyCode.None 
            || key == KeyCode.Escape || key == KeyCode.Return 
            || key == KeyCode.Mouse0 || key == KeyCode.Mouse1 || key == KeyCode.Mouse2
            || key == KeyCode.Z || key == KeyCode.Q || key == KeyCode.S || key == KeyCode.D
        );
    }
    private void TickKeyInput()
    {
        if (_clockTick >= _tick) {
            _clockTick = 0;
            if (_currentColor == "#ffffff00") {
                _currentColor = _keyColor;
            } else {
                _currentColor = "#ffffff00";
            }
        }
        _KeyInputText.text = "[ <color=" + _currentColor + ">" + _currentKey.ToString() + "</color> ]";
        _clockTick += Time.deltaTime;
    }

    private void LoadSlotsUI()
    {
        uint nbPowerUnlocked = 0;
        uint nbSlots = 0;

        for (int idx1 = 0; idx1 < _powerManager._powers.Count; ++idx1)
        {
            if (_powerManager._powers[idx1].unlocked) {
                ++nbPowerUnlocked;
            }
        }
        if (nbPowerUnlocked >= _powerGUiManger.maxSlots) {
            nbSlots = _powerGUiManger.maxSlots;
        } else {
            nbSlots = nbPowerUnlocked;
        }

        for (uint idx = 0; idx < nbSlots; ++idx)
        {
            GameObject newSlotUi = UnityEngine.Object.Instantiate(_prefabSlotButton, _slotPanel.transform.position, _slotPanel.transform.rotation, _slotPanel.transform);
            SlotPowerUiButton slotPowerUiButton = newSlotUi.GetComponent<SlotPowerUiButton>();
            slotPowerUiButton.powerIndex = _currentPower.powerIdx;
            slotPowerUiButton.slotIndex = (int)idx;
            slotPowerUiButton.key = _currentKey;
            newSlotUi.transform.GetChild(0).Find("Text").GetComponent<Text>().text = "Slot " + (idx + 1).ToString();
        }
    }
    private void UnLoadSlotsUI()
    {
        var slotPowerUiButtons = FindObjectsOfType<SlotPowerUiButton>();

        foreach (var slotPowerUiButton in slotPowerUiButtons) 
        {
            UnityEngine.Object.Destroy(slotPowerUiButton.gameObject);
        }
    }

    private void ChoosePower()
    {
        Vector2 movePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas.transform as RectTransform,
            Input.mousePosition, _canvas.worldCamera,
            out movePos);
        var mousePos = _canvas.transform.TransformPoint(movePos);

        foreach (var guiPower in _uiPowers) {
            var rectTransform = guiPower.entity.GetComponent<RectTransform>();
            //changer le 1f la ptn
            if (RangeOf(mousePos.x, rectTransform.position.x, 1f) && RangeOf(mousePos.y, rectTransform.position.y, 1f) && guiPower.unlocked) {
                guiPower.powerName.color = Color.yellow;
                guiPower.ImagePowerHightLight.SetActive(true);
                if (Input.GetMouseButtonDown(0)) {
                    guiPower.powerName.color = Color.white;
                    guiPower.ImagePowerHightLight.SetActive(false);
                    _currentPower = guiPower;
                    AbleChoose();
                    CenterPower();
                }
            } else {
                guiPower.powerName.color = Color.white;
                guiPower.ImagePowerHightLight.SetActive(false);
            }
        }
    }

    private void LoadPowerIUSlots()
    {
        for (int idx = 0; idx < _powerManager._powers.Count; ++idx) {

            //meme ordre dans le tableau, ou tcheck le APower du coup
            bool powerUnlock = _powerManager._powers[idx].unlocked;

            if (idx < _uiPowers.Length) {
                _uiPowers[idx].ImagePowerUnLocked.SetActive(powerUnlock);
                _uiPowers[idx].ImagePowerLocked.SetActive(!powerUnlock);
                _uiPowers[idx].unlocked = powerUnlock;
                _uiPowers[idx].powerIdx = idx;
            }
        }
    }

    private IEnumerator MoveAndDisableDialog(Vector3 dest, Transform objectToMove)
    {
        if (_movePannelDescriptionStory != null)
            StopCoroutine(_movePannelDescriptionStory);
        _movePannelDescriptionStory = StartCoroutine(MoveGUI(_posSlotDescriptionStoryBase.position, _posSlotDescriptionStoryFollow.gameObject.transform));
        if (_movePannelDescriptionUsage != null)
            StopCoroutine(_movePannelDescriptionUsage);
        _movePannelDescriptionUsage = StartCoroutine(MoveGUI(_posSlotDescriptionUsageBase.position, _posSlotDescriptionUsageFollow.gameObject.transform));
        
        yield return _movePannelDescriptionStory;
        _descriptionDialogStory.StopDialogue();
        yield return _movePannelDescriptionUsage;
        _descriptionDialogUsage.StopDialogue();
    }

    private IEnumerator MoveAndSetActiveFalseMenu(Vector3 dest, Transform objectToMove)
    {
        if (_moveUiCoroutine != null)
            StopCoroutine(_moveUiCoroutine);
        _moveUiCoroutine = StartCoroutine(MoveGUI(_uiBasePos.position, _powerUi.transform));
        yield return _moveUiCoroutine;
        _powerUi.SetActive(false);
    }

    private IEnumerator MoveGUI(Vector3 dest, Transform objectToMove)
    {
        //Debug.Log("Range x = " + RangeOf(_powerGui.transform.position.x, dest.x, _moveFramePrecision) + "Range y = " + RangeOf(_powerGui.transform.position.y, dest.y, 1.0f));
        while (!RangeOf(objectToMove.position.x, dest.x, _moveFramePrecision) || !RangeOf(objectToMove.position.y, dest.y, _moveFramePrecision)) {
            float xToAdd = 0f;
            float yToAdd = 0f;
            if (!RangeOf(objectToMove.position.x, dest.x, _moveFramePrecision)) {
                if (objectToMove.position.x < dest.x) {
                    xToAdd = _moveFrameDist;
                } else {
                    xToAdd = -_moveFrameDist;
                }
            }
            if (!RangeOf(objectToMove.position.y, dest.y, _moveFramePrecision)) {
                if (objectToMove.position.y < dest.y) {
                    yToAdd = _moveFrameDist;
                } else {
                    yToAdd = -_moveFrameDist;
                }
            }
            objectToMove.position = new Vector3(
                objectToMove.position.x + xToAdd,
                objectToMove.position.y + yToAdd,
                objectToMove.position.z);
            yield return new WaitForSeconds(_moveFrameTime * Time.deltaTime);
        }
        objectToMove.position = dest;
    }

    private bool RangeOf(float posAim, float posCompare, float range)
    {
        return (posCompare - range <= posAim) && (posAim <= posCompare + range);
    }
}