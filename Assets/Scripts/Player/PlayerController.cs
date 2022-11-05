using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum PlayerType : int
{
    PLAYER = 0,
    RAT = 1
}

// 4.5, 0, 6, 5.5
[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
    [Serializable]
    public struct MovementValues {
        public float maxPlayerSpeed;
        public float playerSpeed;
        public float acceleration;
        public float jumpPower;
    }
    [SerializeField] private Transform[] _particles;
    [SerializeField] private MovementValues _playerValues;
    [SerializeField] private MovementValues _ratValues;
    private MovementValues[] _movementValues;
    private bool _blockInput = false;

    private float deceleration = 0.0f;

    private bool _goingLeft = false;

    private bool _isGrounded;

    private Rigidbody2D _rigidBody;

    private Animator _animator;

    private BoxCollider2D _collider;

    private int _levelLayerMask = 0;
    private int _currentValueIndex = 0;
    private const float _groundedRayMagnitude = 0.075f;

    void Start()
    {
        _movementValues = new MovementValues[2];
        _movementValues[0] = _playerValues;
        _movementValues[1] = _ratValues;
        _rigidBody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<BoxCollider2D>();
        deceleration = _movementValues[_currentValueIndex].acceleration * 3.5f;
        _levelLayerMask = (1 << 11);
    }

    public void ChangeValueIndex(PlayerType type)
    {
        _currentValueIndex = (int)type;
    }

    public bool isGrounded()
    {
        Vector2 rayPosition = transform.position;

        rayPosition.x = rayPosition.x - (_collider.size.x * transform.localScale.x) / 2 + _collider.offset.x;
        rayPosition.y = rayPosition.y - (_collider.size.y * transform.localScale.y) / 2 - _groundedRayMagnitude + _collider.offset.y;

        RaycastHit2D hit = Physics2D.Raycast(rayPosition, Vector2.right, _collider.size.x * transform.localScale.x, _levelLayerMask);
        _isGrounded = hit;

        Debug.DrawRay(rayPosition, Vector2.right * _collider.size.x * transform.localScale.x, _isGrounded ? Color.red : Color.green);
        return _isGrounded;
    }

    void FlipPlayerHorizontally()
    {
        var tempScale = transform.localScale;
        tempScale.x *= -1;
        transform.localScale = tempScale;

        foreach (var particle in _particles)
        {
            var scale = particle.localScale;
            scale.x *= -1;
            particle.localScale = scale;
        }
        
        _goingLeft = !_goingLeft;
    }

    void AnimateMovement(bool idle)
    {
        _animator.SetFloat("Y Velocity", _rigidBody.velocity.y);
        _animator.SetBool("Jumping", !_isGrounded);
        _animator.SetBool("Walking", _isGrounded && !idle);
    }

    void Move(float input)
    {
        bool oppositeDirection = _goingLeft && input < 0 || !_goingLeft && input > 0;
        bool oppositeVelocity = _goingLeft && _rigidBody.velocity.x < 0 || !_goingLeft && _rigidBody.velocity.x > 0;

        if (oppositeDirection)
        {
            FlipPlayerHorizontally();
        }

        if ((input > 0 && _movementValues[_currentValueIndex].playerSpeed < _movementValues[_currentValueIndex].maxPlayerSpeed)
            || (input < 0 && _movementValues[_currentValueIndex].playerSpeed > -_movementValues[_currentValueIndex].maxPlayerSpeed))
        {
            if (oppositeDirection)
                _movementValues[_currentValueIndex].playerSpeed = -_movementValues[_currentValueIndex].playerSpeed * 0.8f;
            else
                _movementValues[_currentValueIndex].playerSpeed += _movementValues[_currentValueIndex].acceleration * input * Time.deltaTime;
        }
        else
        {
            if (_movementValues[_currentValueIndex].playerSpeed > deceleration * Time.deltaTime)
                _movementValues[_currentValueIndex].playerSpeed = _movementValues[_currentValueIndex].playerSpeed - deceleration * Time.deltaTime;
            else if (_movementValues[_currentValueIndex].playerSpeed < -deceleration * Time.deltaTime)
                _movementValues[_currentValueIndex].playerSpeed = _movementValues[_currentValueIndex].playerSpeed + deceleration * Time.deltaTime;
            else
                _movementValues[_currentValueIndex].playerSpeed = 0;
        }

    }

    private void PositionHotfix(Vector2 rayPosition, RaycastHit2D hit)
    {
        if (_isGrounded == false)
            return;
        var halfColliderHeight = (_collider.size.y * transform.localScale.y) / 2;

        // TODO: Do only one time per frame the calculation of colliders (size.x * localScale.x) / 2
        var safeRayPosition = transform.position;
        safeRayPosition.x = Mathf.Clamp(hit.point.x, transform.position.x - (_collider.size.x * Mathf.Abs(transform.localScale.x)) / 2 * 0.6f, transform.position.x + (_collider.size.x * Mathf.Abs(transform.localScale.x)) / 2 * 0.6f);
        var verticalHit = Physics2D.Raycast(safeRayPosition, Vector2.down, halfColliderHeight + _groundedRayMagnitude, _levelLayerMask);

        if (!verticalHit)
            return;
        var fixedPosition = transform.position;
        fixedPosition.y -= (transform.position.y - (halfColliderHeight) - verticalHit.point.y);
        transform.position = fixedPosition;
        Debug.DrawRay(safeRayPosition, Vector2.down * (halfColliderHeight + _groundedRayMagnitude), _isGrounded ? Color.red : Color.green);
    }

    private void Jump()
    {
        _rigidBody.velocity = new Vector2(0, _movementValues[_currentValueIndex].jumpPower);
    }

    public void BlockInput(bool block)
    {
        _blockInput = block;

        if (_blockInput)
            _movementValues[_currentValueIndex].playerSpeed = 0;
    }

    void Update()
    {
        
        if (Input.GetButtonDown("Debug Fire"))
            GetComponent<BasicHealthWrapper>().Hit(1);

        if (_animator.GetBool("Dead"))
            _movementValues[_currentValueIndex].playerSpeed = 0;
        else
        {
            bool grounded = isGrounded();
            if (!_blockInput && grounded && Input.GetButtonDown("Jump"))
                Jump();

            float input = Input.GetAxisRaw("Horizontal");

            if (_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "PlayerHit")
                _movementValues[_currentValueIndex].playerSpeed = 0;
            else if (!_blockInput)
                Move(input);

            AnimateMovement(input == 0 | _blockInput);
        }
    }

    private void FixedUpdate()
    {
        _rigidBody.velocity = new Vector2(_movementValues[_currentValueIndex].playerSpeed, _rigidBody.velocity.y);
    }
}
