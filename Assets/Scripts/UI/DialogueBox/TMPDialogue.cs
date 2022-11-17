using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using ExtensionMethods;

public class TMPDialogue : MonoBehaviour
{
    public enum TextColor
    {
        SOLID,
        HORIZONTAL_GRADIENT,
        VERTICAL_GRADIENT
    }
    public enum TextModifier
    {
        NONE,
        WODDLE,
        WAVE,
        RAND_BOUNCE,
        BOUNCE
    }

    [Serializable]
    public struct ModifiableText
    {
        public string _text;
        public TextModifier _textModifier;
        public TextColor _colorModifier;
        public Color32 _solidColor;
        public Color32 _gradientColor;
        [Range(0.0f, 1.0f)] public float _colorSpeedRandomness;
        [HideInInspector] public int _firstIndex;
        [HideInInspector] public int _lastIndex;
        [HideInInspector] public List<float> _colorRandomSpeed;
    }

    [Serializable]
    public struct DialoguePartOfSide
    {
        public ModifiableText[] _modifiableText;
        [HideInInspector] public string _fullText;
    }

    [Serializable]
    public struct Dialogue
    {
        public string dialogueName;
        public bool _setupOnStart;
        [HideInInspector] public bool _isSetup;
        public DialoguePartOfSide[] _partOfSide;
    }

    [SerializeField] protected Dialogue[] _dialogues;
    [SerializeField] private GameObject _dialogueBox;
    [SerializeField] private Transform _canvasTransform;
    [SerializeField] private float _waveSpeed;
    [SerializeField] private float _waveAmplitude;
    [SerializeField] private Vector2 _bounceSpeed;
    [SerializeField] private Vector2 _bounceAmplitude;
    [SerializeField] private Vector2 _woodleSpeed;
    [SerializeField] private Vector2 _woodleAmplitude;
    [SerializeField] private float _popDuration;
    [SerializeField] private bool _handleByInput = false;
    [SerializeField] private Transform _target;
    [SerializeField] private Vector2 _targetOffset;
    private SoundEffect _soundEffectPlayer;
    protected TMP_Text _textMeshPro;
    protected Coroutine _popCoroutine = null;
    protected Mesh _mesh;
    protected Vector3[] _vertices;
    protected Color32[] _colors;
    protected int _dialogueIndex;
    protected int _partOfSideIndex;
    protected bool _isCompute = false;
    protected float _currentPopSpeed;
    protected bool _canSkip = false;
    protected GameObject _dialogueBoxReference = null;

    private void Start()
    {
        _soundEffectPlayer = GetComponent<SoundEffect>();
        _currentPopSpeed = _popDuration;
        for (int i = 0; i < _dialogues.Length; ++i)
        {
            _dialogues[i]._isSetup = false;
            if (_dialogues[i]._setupOnStart)
                SetupDialogue(i);
        }
    }

    public void SetCanvasTransform(Transform canvasTransform)
    {
        _canvasTransform = canvasTransform;
    }

    public bool HasFinishPop()
    {
        return (_popCoroutine == null);
    }

    public virtual bool IsFinish()
    {
        return (_dialogueBoxReference == null);
    }

    protected void ChangeText(int partOfSideIndex)
    {
        _textMeshPro.text = _dialogues[_dialogueIndex]._partOfSide[partOfSideIndex]._fullText;
        _partOfSideIndex = partOfSideIndex;
    }

    protected void SetupDialogue(int dialogueIndex)
    {
        int index = 0;
        _dialogues[dialogueIndex]._isSetup = true;
        for (int j = 0; j < _dialogues[dialogueIndex]._partOfSide.Length; ++j, index = 0)
        {
            for (int i = 0; i < _dialogues[dialogueIndex]._partOfSide[j]._modifiableText.Length; ++i)
            {
                _dialogues[dialogueIndex]._partOfSide[j]._modifiableText[i]._colorRandomSpeed = new List<float>();

                if (_dialogues[dialogueIndex]._partOfSide[j]._modifiableText[i]._text.Contains("$name$"))
                {
                    _dialogues[dialogueIndex]._partOfSide[j]._modifiableText[i]._text = _dialogues[dialogueIndex]._partOfSide[j]._modifiableText[i]._text.Replace("$name$", SaveManager.DataInstance.GetPlayerInfo()._playerName);
                }

                if (_dialogues[dialogueIndex]._partOfSide[j]._modifiableText[i]._text.Contains("\\n"))
                {
                    _dialogues[dialogueIndex]._partOfSide[j]._modifiableText[i]._text = _dialogues[dialogueIndex]._partOfSide[j]._modifiableText[i]._text.Replace("\\n", "\n");
                }

                foreach (char c in _dialogues[dialogueIndex]._partOfSide[j]._modifiableText[i]._text)
                {
                    if (c != ' ' && c != '\n')
                    {
                        float random = UnityEngine.Random.Range(0.0f, 1.0f * _dialogues[dialogueIndex]._partOfSide[j]._modifiableText[i]._colorSpeedRandomness);
                        _dialogues[dialogueIndex]._partOfSide[j]._modifiableText[i]._colorRandomSpeed.Add(1.0f - random);
                    }
                }
                int substringToDeduce = _dialogues[dialogueIndex]._partOfSide[j]._modifiableText[i]._text.CountSubstring("<i>") * 3 + _dialogues[dialogueIndex]._partOfSide[j]._modifiableText[i]._text.CountSubstring("</i>") * 4;
                _dialogues[dialogueIndex]._partOfSide[j]._modifiableText[i]._firstIndex = index;
                _dialogues[dialogueIndex]._partOfSide[j]._modifiableText[i]._lastIndex = index + _dialogues[dialogueIndex]._partOfSide[j]._modifiableText[i]._text.Length - substringToDeduce;
                index = _dialogues[dialogueIndex]._partOfSide[j]._modifiableText[i]._lastIndex;
                _dialogues[dialogueIndex]._partOfSide[j]._fullText += _dialogues[dialogueIndex]._partOfSide[j]._modifiableText[i]._text;
            }
        }
    }

    private void ColorGradient(int vertexIndex, ModifiableText d, int colorIndex)
    {
        if (d._colorModifier == TextColor.SOLID)
            return;
        _colors[vertexIndex] = Color32.Lerp(d._solidColor, d._gradientColor, Mathf.PingPong(Time.time * d._colorRandomSpeed[colorIndex] - (d._colorModifier == TextColor.HORIZONTAL_GRADIENT ? _vertices[vertexIndex].x : _vertices[vertexIndex].y) * 0.01f, 1.0f));
        _colors[vertexIndex + 1] = Color32.Lerp(d._solidColor, d._gradientColor, Mathf.PingPong(Time.time * d._colorRandomSpeed[colorIndex] - (d._colorModifier == TextColor.HORIZONTAL_GRADIENT ? _vertices[vertexIndex + 1].x : _vertices[vertexIndex + 1].y)* 0.01f, 1.0f));
        _colors[vertexIndex + 2] = Color32.Lerp(d._solidColor, d._gradientColor, Mathf.PingPong(Time.time * d._colorRandomSpeed[colorIndex] - (d._colorModifier == TextColor.HORIZONTAL_GRADIENT ? _vertices[vertexIndex + 2].x : _vertices[vertexIndex + 2].y)* 0.01f, 1.0f));
        _colors[vertexIndex + 3] = Color32.Lerp(d._solidColor, d._gradientColor, Mathf.PingPong(Time.time * d._colorRandomSpeed[colorIndex] - (d._colorModifier == TextColor.HORIZONTAL_GRADIENT ? _vertices[vertexIndex + 3].x : _vertices[vertexIndex + 3].y)* 0.01f, 1.0f));
    }

    private bool PopCharacter(int vertexIndex, float time, TMP_CharacterInfo info)
    {
        Vector3 center = new Vector3(info.topLeft.x + (info.topRight.x - info.topLeft.x) * 0.5f, info.bottomLeft.y + (info.topRight.y - info.bottomRight.y) * 0.5f, 1);

        _vertices[vertexIndex] = Vector3.Lerp(center, info.bottomLeft, time);
        _vertices[vertexIndex + 1] = Vector3.Lerp(center, info.topLeft, time);
        _vertices[vertexIndex + 2] = Vector3.Lerp(center, info.topRight, time);
        _vertices[vertexIndex + 3] = Vector3.Lerp(center, info.bottomRight, time);
            
        if (_vertices[vertexIndex] == info.bottomLeft && _vertices[vertexIndex + 1] == info.topLeft
            && _vertices[vertexIndex + 2] == info.topRight && _vertices[vertexIndex + 3] == info.bottomRight)
        {
            return true;
        }
        return false;
    }

    protected virtual IEnumerator PopDialogue(int dialogueIndex)
    {
        ChangeText(dialogueIndex);
        _isCompute = false;
        float time = 0.0f;
        bool updateI = false;

        for (int i = 0; i < _textMeshPro.text.Length; updateI = false)
        {
            _textMeshPro.ForceMeshUpdate();

            _mesh = _textMeshPro.mesh;
            _vertices = _mesh.vertices;
            _colors = _mesh.colors32;

            foreach (var d in _dialogues[_dialogueIndex]._partOfSide[_partOfSideIndex]._modifiableText)
            {
                for (int inc = d._firstIndex, j = 0; inc < d._lastIndex; ++inc)
                {
                    TMP_CharacterInfo info = _textMeshPro.textInfo.characterInfo[inc];

                    int vertexIndex = info.vertexIndex;

                    if (info.character == ' '|| info.character == '\n')
                    {
                        if (inc == i)
                        {
                            i++;
                            updateI = true;
                            time = 0.0f;
                        }
                        continue;
                    }

                    if (inc == i)
                    {
                        if (Time.deltaTime < _currentPopSpeed)
                        {
                            time += Time.deltaTime / _currentPopSpeed;

                            if ((updateI = PopCharacter(vertexIndex, time, info)))
                            {
                                if (_soundEffectPlayer != null) {
                                    if (_currentPopSpeed == _popDuration)
                                        _soundEffectPlayer.PlaySound(SoundData.SoundEffectName.UI_CHARACTER_POP);
                                    else if (i % 4 == 0)
                                        _soundEffectPlayer.PlaySound(SoundData.SoundEffectName.UI_CHARACTER_POP);
                                }
                                i++;
                                time = 0.0f;
                                
                            }                            
                        }
                        else
                            updateI = true;
                    }

                    if (inc < i || (inc == i && !updateI))
                    {
                        UpdateTextModifier(vertexIndex, d, inc);
                        UpdateColor(vertexIndex, d, j);
                    }
                    else if ((inc == i && updateI) || inc > i)
                    {
                        Vector3 center = new Vector3(info.topLeft.x + (info.topRight.x - info.topLeft.x) * 0.5f, info.bottomLeft.y + (info.topRight.y - info.bottomRight.y) * 0.5f, 1);

                        _vertices[vertexIndex] = center;
                        _vertices[vertexIndex + 1] = center;
                        _vertices[vertexIndex + 2] = center;
                        _vertices[vertexIndex + 3] = center;
                    }
                    ++j;
                }
            }
            _mesh.colors32 = _colors;
            _mesh.vertices = _vertices;
            FollowTarget();
            yield return null;
        }
        _isCompute = true;
        _popCoroutine = null;
    }

    protected void UpdateTextModifier(int vertexIndex, ModifiableText d, int currentIndex)
    {
        if (d._textModifier == TextModifier.WODDLE)
        {
            Vector3 offset = Wobble(Time.time + currentIndex, _woodleSpeed, _woodleAmplitude);
            _vertices[vertexIndex] += offset;
            _vertices[vertexIndex + 1] += offset;
            _vertices[vertexIndex + 2] += offset;
            _vertices[vertexIndex + 3] += offset;
        }
        else if (d._textModifier == TextModifier.WAVE)
        {
            float offset = CosWave(Time.time + currentIndex);
            _vertices[vertexIndex].y += offset;
            _vertices[vertexIndex + 1].y += offset;
            _vertices[vertexIndex + 2].y += offset;
            _vertices[vertexIndex + 3].y += offset;
        }
        else if (d._textModifier == TextModifier.RAND_BOUNCE)
        {
            Vector3 offset = Wobble(Time.time + currentIndex, _bounceSpeed, _bounceAmplitude);
            _vertices[vertexIndex].x -= offset.x;
            _vertices[vertexIndex].y -= offset.y;
            _vertices[vertexIndex + 1].x -= offset.x;
            _vertices[vertexIndex + 1].y += offset.y;
            _vertices[vertexIndex + 2].x += offset.x;
            _vertices[vertexIndex + 2].y += offset.y;
            _vertices[vertexIndex + 3].x += offset.x;
            _vertices[vertexIndex + 3].y -= offset.y;
        }
        else if (d._textModifier == TextModifier.BOUNCE)
        {
            Vector3 offset = Wobble(Time.time, _bounceSpeed, _bounceAmplitude);
            _vertices[vertexIndex].x -= offset.x;
            _vertices[vertexIndex].y -= offset.y;
            _vertices[vertexIndex + 1].x -= offset.x;
            _vertices[vertexIndex + 1].y += offset.y;
            _vertices[vertexIndex + 2].x += offset.x;
            _vertices[vertexIndex + 2].y += offset.y;
            _vertices[vertexIndex + 3].x += offset.x;
            _vertices[vertexIndex + 3].y -= offset.y;
        }
    }

    protected void UpdateColor(int vertexIndex, ModifiableText d, int colorIndex)
    {
        if (d._colorModifier == TextColor.SOLID)
        {
            _colors[vertexIndex] = d._solidColor;
            _colors[vertexIndex + 1] = d._solidColor;
            _colors[vertexIndex + 2] = d._solidColor;
            _colors[vertexIndex + 3] = d._solidColor;
        }
        else if (d._colorModifier == TextColor.HORIZONTAL_GRADIENT || d._colorModifier == TextColor.VERTICAL_GRADIENT)
        {
            ColorGradient(vertexIndex, d, colorIndex);
        }
    }

    protected void InstantiateDialogueBox()
    {
        _dialogueBoxReference = Instantiate(_dialogueBox, _canvasTransform) as GameObject;
        _textMeshPro = _dialogueBoxReference.transform.GetChild(0).GetComponent<TMP_Text>();
        if (_textMeshPro == null)
            Debug.Log("No tmp found");
        FollowTarget();
    }

    public virtual void StartDialogue(int index)
    {
        if (_popCoroutine != null)
        {
            StopCoroutine(_popCoroutine);
            _popCoroutine = null;
        }
        if (index >= _dialogues.Length)
        {
            Debug.Log("This dialogue doesn't exist");
            return;
        }

        if (_dialogueBoxReference == null)
            InstantiateDialogueBox();

        _canSkip = true;
        _dialogueIndex = index;

        if (!_dialogues[_dialogueIndex]._isSetup)
            SetupDialogue(_dialogueIndex);

        _partOfSideIndex = 0;
        if (_popDuration == 0.0f)
        {
            ChangeText(_partOfSideIndex);
            _isCompute = true;
        }
        else
            _popCoroutine = StartCoroutine(PopDialogue(_partOfSideIndex));
    }

    protected int? GetDialogueByName(string dialogueName)
    {
        int i = 0;
        foreach (Dialogue d in _dialogues)
        {
            if (d.dialogueName == dialogueName)
                return i;
            ++i;
        }
        return null;
    }

    public virtual void StartDialogue(string dialogueName)
    {
        if (_popCoroutine != null)
        {
            StopCoroutine(_popCoroutine);
            _popCoroutine = null;
        }
        int? d = GetDialogueByName(dialogueName);
        if (d == null)
        {
            Debug.Log("This dialogue doesn't exist");
            return;
        }

        if (_dialogueBoxReference == null)
            InstantiateDialogueBox();
        _canSkip = true;
        _dialogueIndex = (int)d;

        if (!_dialogues[_dialogueIndex]._isSetup)
            SetupDialogue(_dialogueIndex);

        _partOfSideIndex = 0;
        if (_popDuration == 0.0f)
        {
            ChangeText(_partOfSideIndex);
            _isCompute = true;
        }
        else
            _popCoroutine = StartCoroutine(PopDialogue(_partOfSideIndex));
    }

    public void StopDialogue()
    {
        _isCompute = false;
        if (_popCoroutine != null)
            StopCoroutine(_popCoroutine);
        _popCoroutine = null;
        if (_dialogueBoxReference != null)
            Destroy(_dialogueBoxReference);
        _dialogueBoxReference = null;
        _textMeshPro = null;
    }

    private void UpdateInput()
    {
        if (_dialogueBoxReference == null)
            return;
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (_popCoroutine == null)
            {
                if (_partOfSideIndex + 1 >= _dialogues[_dialogueIndex]._partOfSide.Length)
                {
                    StopDialogue();
                }
                else
                {
                    _canSkip = false;
                    if (_popDuration == 0.0f)
                    {
                        ChangeText(_partOfSideIndex + 1);
                        _isCompute = true;
                    }
                    else
                        _popCoroutine = StartCoroutine(PopDialogue(_partOfSideIndex + 1));
                }
            }
        }
        else if (Input.GetKey(KeyCode.Return) && _canSkip)
        {
            _currentPopSpeed = _popDuration * 0.3f;
        }

        if (Input.GetKeyUp(KeyCode.Return))
        {
            if (!_canSkip)
                _canSkip = true;
            _currentPopSpeed = _popDuration;
        }
    }

    private void Update()
    {
        if (_handleByInput)
            UpdateInput();

        if (!_isCompute)
            return;

        _textMeshPro.ForceMeshUpdate();
        _mesh = _textMeshPro.mesh;
        _vertices = _mesh.vertices;
        _colors = _mesh.colors32;

        foreach (var d in _dialogues[_dialogueIndex]._partOfSide[_partOfSideIndex]._modifiableText)
        {
            for (int i = d._firstIndex, j = 0; i < d._lastIndex; ++i)
            {
                TMP_CharacterInfo info = _textMeshPro.textInfo.characterInfo[i];

                if (info.character == ' ' || info.character == '\n')
                    continue;

                int vertexIndex = info.vertexIndex;

                UpdateTextModifier(vertexIndex, d, i);
                UpdateColor(vertexIndex, d, j);

                ++j;
            }
        }
        _mesh.vertices = _vertices;
        _mesh.colors32 = _colors;

        FollowTarget();
    }

    public string[] GetDialogueNames()
    {
        string[] dialogNames = new string[_dialogues.Length]; 
        int index = 0;
        
        foreach (var dialog in _dialogues) {
            dialogNames[index] = dialog.dialogueName;
            ++index;
        }
        return dialogNames;
    }

    private void FollowTarget()
    {
        if (_target != null)
        {
            var canvasRect = _canvasTransform.gameObject.GetComponent<RectTransform>();
            Vector2 viewportPosition = Camera.main.WorldToViewportPoint(_target.transform.position + (Vector3)_targetOffset);
            Vector2 worldObject_ScreenPosition = new Vector2(
            ((viewportPosition.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)),
            ((viewportPosition.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)));

            if (_dialogueBoxReference != null)
                _dialogueBoxReference.GetComponent<RectTransform>().anchoredPosition = worldObject_ScreenPosition;
        }
    }

    public void SetUpTarget(Transform target, Vector3? offset = null)
    {
        _target = target;
        _targetOffset = offset ?? Vector3.zero;
    }

    private float CosWave(float time)
    {
        return Mathf.Cos(time * _waveSpeed) * _waveAmplitude;
    }

    private Vector2 Wobble(float time, Vector2 speed, Vector2 amplitude)
    {
        return new Vector2(Mathf.Sin(time * speed.x) * amplitude.x, Mathf.Cos(time * speed.y) * amplitude.y);
    }
}
