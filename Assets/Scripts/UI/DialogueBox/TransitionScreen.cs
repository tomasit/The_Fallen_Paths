using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TransitionScreen : TMPDialogue
{
    public enum TransitionState {
        BEGIN,
        MIDDLE,
        END,
        NONE
    }
    [SerializeField] private float _fadeDuration;
    [SerializeField] private float _waitTime;
    private TransitionState _transitionState = TransitionState.NONE;
    private Coroutine _transitionCoroutine = null;
    private bool playSpecialQuote = false;

    public TransitionState GetTransitionState()
    {
        return _transitionState;
    }

    public void StartDeadTransition()
    {
        string quoteName = null;
        if (playSpecialQuote)
        {
            quoteName = "SpecialQuote_1.1";
            playSpecialQuote = false;            
        }
        else
        {
            int maxQuote = 7;
            int seed = Random.Range(1, maxQuote + 1);
            quoteName = (seed == maxQuote ? "SpecialQuote_1.0" : ("Quote_" + seed));
            playSpecialQuote = (seed == maxQuote ? true : false);
        }

        StartDialogue(quoteName);

        var c = _dialogueBoxReference.GetComponent<Image>().color;
        c.a = 0.0f;
        _dialogueBoxReference.GetComponent<Image>().color = c;
        
        _transitionCoroutine = StartCoroutine(FullTransition());
    }

    private IEnumerator PopAlphaCharacter(float fadeDuration)
    {
        ChangeText(_partOfSideIndex);
        _transitionState = TransitionState.BEGIN;
        _isCompute = false;
        
        for (float t = 0, f = 0.0f; f < 1.0f; t += Time.deltaTime / fadeDuration)
        {
            UpdateAlpha((f = Mathf.Lerp(0.0f, 1.0f, t)));
            yield return null;
        }
        _popCoroutine = null;
        _transitionState = TransitionState.MIDDLE;
    }

    private IEnumerator SemiTransition()
    {
        if (_popCoroutine != null)
            StopCoroutine(_popCoroutine);
        yield return (_popCoroutine = StartCoroutine(PopAlphaCharacter(2.0f)));
        _isCompute = true;
        yield return new WaitForSeconds(_waitTime);
        yield return (_popCoroutine = StartCoroutine(DepopDialogue()));
        StopDialogue();
        _transitionCoroutine = null;
    }

    public void StartBeginTransition(string quoteName)
    {
        StartDialogue(quoteName);
        _dialogueBoxReference.GetComponent<Image>().color = Color.black;
        _transitionCoroutine = StartCoroutine(SemiTransition());
    }

    public void StartTransition(string quoteName)
    {
        StartDialogue(quoteName);
        var c = _dialogueBoxReference.GetComponent<Image>().color;
        c.a = 0.0f;
        _dialogueBoxReference.GetComponent<Image>().color = c;
        
        _transitionCoroutine = StartCoroutine(FullTransition());
    }

    private void UpdateAlpha(float alpha)
    {
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

                _colors[vertexIndex].a = (byte)(alpha * 255);
                _colors[vertexIndex + 1].a = (byte)(alpha * 255);
                _colors[vertexIndex + 2].a = (byte)(alpha * 255);
                _colors[vertexIndex + 3].a = (byte)(alpha * 255);
                ++j;
            }
        }
        _mesh.vertices = _vertices;
        _mesh.colors32 = _colors;
    }

    protected override IEnumerator PopDialogue(int dialogueIndex)
    {
        ChangeText(dialogueIndex);
        _transitionState = TransitionState.BEGIN;
        _isCompute = false;

        Color baseImgColor = _dialogueBoxReference.GetComponent<Image>().color;
        Color nextImgColor = new Color(baseImgColor.r, baseImgColor.g, baseImgColor.b, 1.0f);

        for (float t = 0; _dialogueBoxReference.GetComponent<Image>().color != nextImgColor; t += Time.deltaTime / _fadeDuration) //|| _textMeshPro.color != nextTxtColor; t += Time.deltaTime / _fadeDuration)
        {
            _dialogueBoxReference.GetComponent<Image>().color = Color.Lerp(baseImgColor, nextImgColor, t);

            UpdateAlpha(_dialogueBoxReference.GetComponent<Image>().color.a);
            yield return null;
        }
        _popCoroutine = null;
        _transitionState = TransitionState.MIDDLE;
    }

    private IEnumerator DepopDialogue()
    {
        _isCompute = false;
        _transitionState = TransitionState.END;
        Color baseImgColor = _dialogueBoxReference.GetComponent<Image>().color;
        Color nextImgColor = new Color(baseImgColor.r, baseImgColor.g, baseImgColor.b, 0.0f);

        for (float t = 0; _dialogueBoxReference.GetComponent<Image>().color != nextImgColor; t += Time.deltaTime / _fadeDuration) //|| _textMeshPro.color != nextTxtColor; t += Time.deltaTime / _fadeDuration)
        {
            _dialogueBoxReference.GetComponent<Image>().color = Color.Lerp(baseImgColor, nextImgColor, t);
            UpdateAlpha(_dialogueBoxReference.GetComponent<Image>().color.a);
            yield return null;
        }
        _transitionState = TransitionState.NONE;
    }

    private IEnumerator FullTransition()
    {
        if (_popCoroutine != null)
            StopCoroutine(_popCoroutine);
        yield return (_popCoroutine = StartCoroutine(PopDialogue(_partOfSideIndex)));
        _isCompute = true;
        yield return new WaitForSeconds(_waitTime);
        yield return (_popCoroutine = StartCoroutine(DepopDialogue()));
        StopDialogue();
        _transitionCoroutine = null;
    }

    public override void StartDialogue(int index)
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
        if (_transitionCoroutine != null)
        {
            StopCoroutine(_transitionCoroutine);
            StopDialogue();
        }
    }

    public override void StartDialogue(string dialogueName)
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
        if (_transitionCoroutine != null)
        {
            StopCoroutine(_transitionCoroutine);
            StopDialogue();
        }
    }
}
