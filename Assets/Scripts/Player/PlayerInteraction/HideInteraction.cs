using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideInteraction : MonoBehaviour
{
    private PlayerController _controller;
    private Animator _animator;
    private Coroutine _litCoroutine = null;
    private SpriteRenderer _spriteRenderer;
    [SerializeField] private float _litDuration = 0.3f;
    private bool _isHide = false;

    private void Start()
    {
        _controller = GetComponent<PlayerController>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public bool IsHide()
    {
        return _isHide;
    }

    public void Hide(bool hide, bool isLitPlace)
    {
        _isHide = hide;
        _controller.BlockInput(hide);

        if (isLitPlace && !hide)
        {
            if (_litCoroutine != null)
            {
                StopCoroutine(_litCoroutine);
                _litCoroutine = null;
            }
            _litCoroutine = StartCoroutine(LitCoroutine(_spriteRenderer, false, new Color(1.0f, 1.0f, 1.0f, 1.0f)));
        }
    }

    private IEnumerator LitCoroutine(SpriteRenderer spr, bool isAlight, Color litColor)
    {
        Color baseColor = spr.material.color;

        yield return new WaitForSeconds(Time.deltaTime);

        for (float t = 0; t < 1.0f; t += Time.deltaTime / _litDuration)
        {
            spr.material.color = Color.Lerp(baseColor, litColor , t);
            yield return null;
        }

        _litCoroutine = null;
    }

    public void Cancel()
    {
        _isHide = false;
        if (_litCoroutine != null)
        {
            StopCoroutine(_litCoroutine);
            _litCoroutine = null;
        }
        _spriteRenderer.material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

    public void LitPlaceAlight(bool isAlight)
    {
        if (_litCoroutine != null)
        {
            StopCoroutine(_litCoroutine);
            _litCoroutine = null;
        }
        _litCoroutine = StartCoroutine(LitCoroutine(_spriteRenderer, isAlight, (isAlight ? new Color(.6f, .6f, .6f, 1.0f) : new Color(.0f, .0f, .0f, 1.0f))));
    }
}
