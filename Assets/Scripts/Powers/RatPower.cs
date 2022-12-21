using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatPower : APower
{
    [SerializeField] private SpriteMask _rescaleMask;
    [SerializeField] private SpriteRenderer _yellowSpr;
    [SerializeField] private float _transformationSpeed = 60.0f;
    [SerializeField] private RuntimeAnimatorController _ratAnimator;
    private RuntimeAnimatorController _baseAnimator;
    private Vector3 _baseScale;
    private Vector3 _playerBaseScale;
    private Coroutine _ratCoroutine = null;
    private bool _isRat = false;

    protected override void Start()
    {
        base.Start();
        _playerBaseScale = transform.localScale;
        _baseAnimator = GetComponent<Animator>().runtimeAnimatorController;
        _baseScale = _rescaleMask.transform.localScale;
        _rescaleMask.alphaCutoff = 0.0f;
        _yellowSpr.enabled = false;
    }

    public override void Cancel()
    {

    }

    private IEnumerator RatTransformation(Vector3 target)
    {
        _isRat = !_isRat;
        _rescaleMask.alphaCutoff = 1.0f;
        _yellowSpr.enabled = true;
        firingPower = !firingPower;
        while (_rescaleMask.transform.localScale != target)
        {
            _rescaleMask.transform.localScale = Vector3.MoveTowards(_rescaleMask.transform.localScale, target, _transformationSpeed * Time.deltaTime);
            yield return null;
        }
        _rescaleMask.alphaCutoff = 0.0f;
        _yellowSpr.enabled = false;
        GetComponent<PlayerController>().ChangeValueIndex(_isRat ? PlayerType.RAT : PlayerType.PLAYER);
        GetComponent<SpriteRenderer>().maskInteraction = _isRat ? SpriteMaskInteraction.None : SpriteMaskInteraction.VisibleInsideMask;
        transform.localScale = _isRat ? new Vector3(2 * Mathf.Sign(transform.localScale.x), 2, 1) : new Vector3(_playerBaseScale.x * Mathf.Sign(transform.localScale.x), _playerBaseScale.y, _playerBaseScale.z);
        GetComponent<Animator>().runtimeAnimatorController = (_isRat ? _ratAnimator : _baseAnimator);
        _ratCoroutine = null;
        if (!_isRat)
        {
            var oldPos = transform.position;
            oldPos.y += GetComponent<SpriteRenderer>().bounds.size.y * 0.5f;
            transform.position = oldPos;
        }
    }

    public override void Use()
    {
        if (_ratCoroutine != null)
        {
            StopCoroutine(_ratCoroutine);
            _ratCoroutine = null;
        }

        _ratCoroutine = StartCoroutine(RatTransformation((!_isRat ? Vector3.zero : _baseScale)));
    }

    public override void Fire()
    {
        
    }

    private void Update()
    {
    }
}
