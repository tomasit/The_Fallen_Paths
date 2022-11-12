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
    [SerializeField] private LayerMask _levelLayerMask;
    private MovementValues[] _movementValues;
    private bool _blockInput = false;
    private float deceleration = 0.0f;
    private bool _goingLeft = false;
    private bool _goingTop = false;
    private bool _isGrounded;
    private Rigidbody2D _rigidBody;
    private Animator _animator;
    private BoxCollider2D _collider;
    private int _currentValueIndex = 0;
    private const float _groundedRayMagnitude = 0.075f;
    private float _runtimeGroundedRayMagnitude;
    private bool _isClimbing = false;
    private bool _collideWithLadder = false;
    private float _verticalPlayerSpeed = 0.0f;

    void Start()
    {
        _runtimeGroundedRayMagnitude = _groundedRayMagnitude;
        _movementValues = new MovementValues[2];
        _movementValues[0] = _playerValues;
        _movementValues[1] = _ratValues;
        _rigidBody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<BoxCollider2D>();
        deceleration = _movementValues[_currentValueIndex].acceleration * 3.5f;
        
    }

    public void ChangeValueIndex(PlayerType type)
    {
        _currentValueIndex = (int)type;
    }

    public bool isGrounded()
    {
        Vector2 rayPosition = transform.position;

        rayPosition.x = rayPosition.x - (_collider.size.x * transform.localScale.x) / 2;// + (_collider.offset.x * transform.localScale.x);
        rayPosition.y = rayPosition.y - (_collider.size.y * transform.localScale.y) / 2 - _runtimeGroundedRayMagnitude + (_collider.offset.y * transform.localScale.y);

        RaycastHit2D hit = Physics2D.Raycast(rayPosition, Vector2.right, _collider.size.x * transform.localScale.x, _levelLayerMask);
        _isGrounded = hit;

        Debug.DrawRay(rayPosition, Vector2.right * (_collider.size.x * transform.localScale.x), _isGrounded ? Color.red : Color.green);
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

    void FlipPlayerHorizontally(float mult)
    {
        var tempScale = transform.localScale;
        tempScale.x = Mathf.Abs(tempScale.x) * mult;
        transform.localScale = tempScale;

        foreach (var particle in _particles)
        {
            var scale = particle.localScale;
            scale.x = Mathf.Abs(scale.x) * mult;
            particle.localScale = scale;
        }
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

        if (oppositeDirection)
        {
            FlipPlayerHorizontally();
        }

        HorizontalMovement(input, oppositeDirection);
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

    private void ActiveLadder(bool active)
    {
        _isClimbing = active;
        _rigidBody.gravityScale = active ? 0.0f : 1.0f;
        _rigidBody.velocity = new Vector2(active ? 0 : _rigidBody.velocity.x,
            active || _collideWithLadder ? 0 : 3.0f);
        _movementValues[_currentValueIndex].playerSpeed = (active ? 0.0f : _movementValues[_currentValueIndex].playerSpeed);
        _verticalPlayerSpeed = 0.0f;
        _animator.SetBool("Climbing", _isClimbing);
        if (!active)
        {
            FlipPlayerHorizontally(_goingLeft ? -1 : 1);
            _runtimeGroundedRayMagnitude = _groundedRayMagnitude;
        }
        else
        {
            if (_isGrounded)
                _runtimeGroundedRayMagnitude = 0.0f;
        }
    }

    private void OnTriggerEnter2D(Collider2D hit)
    {
        _collideWithLadder = (hit.gameObject.layer == LayerMask.NameToLayer("PlayerLadder"));
    }

    private void OnTriggerExit2D(Collider2D hit)
    {
        if (hit.gameObject.layer == LayerMask.NameToLayer("PlayerLadder"))
        {
            _collideWithLadder = false;
            if (_isClimbing)
                ActiveLadder(false);
        }
    }

    private void HorizontalMovement(float input, bool oppositeDirection)
    {
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

    private void VerticalMovement(float input)
    {
        if ((input > 0 && _verticalPlayerSpeed < _movementValues[_currentValueIndex].maxPlayerSpeed)
            || (input < 0 && _verticalPlayerSpeed > -_movementValues[_currentValueIndex].maxPlayerSpeed))
        {
            if (input > 0 && _rigidBody.velocity.y < 0)
                _verticalPlayerSpeed = -_movementValues[_currentValueIndex].playerSpeed * 0.8f;
            else
                _verticalPlayerSpeed += _movementValues[_currentValueIndex].acceleration * input * Time.deltaTime;
        }
        else
        {
            if (_goingTop && input > 0)
                _verticalPlayerSpeed = _movementValues[_currentValueIndex].maxPlayerSpeed;
            else if (!_goingTop && input < 0)
                _verticalPlayerSpeed = -_movementValues[_currentValueIndex].maxPlayerSpeed;
            else if (input == 0)
                _verticalPlayerSpeed = 0;
        }
    }

    private void LadderMovement()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        bool oppositeDirection = _goingLeft && horizontalInput < 0 || !_goingLeft && horizontalInput > 0;
        if (oppositeDirection)
            _goingLeft = !_goingLeft;
        float verticalInput = Input.GetAxisRaw("Vertical");
        _goingTop = (verticalInput > 0);

        VerticalMovement(verticalInput);
        HorizontalMovement(horizontalInput, oppositeDirection);

        if (horizontalInput != 0.0f || verticalInput != 0.0f)
            _animator.speed = 1.0f;
        else
            _animator.speed = 0.0f;
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

            if (_collideWithLadder && !_isClimbing && Input.GetAxisRaw("Vertical") > 0.0f)
                ActiveLadder(true);
            else if ((grounded && _isClimbing) || (_isClimbing && Input.GetAxisRaw("Vertical") <= 0.0f))
                ActiveLadder(false);

            if (_isClimbing)
            {
                LadderMovement();
            }
            else
            {
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
    }

    private void FixedUpdate()
    {
        if (_isClimbing)
            _rigidBody.velocity = new Vector2(_movementValues[_currentValueIndex].playerSpeed, _verticalPlayerSpeed);
        else
            _rigidBody.velocity = new Vector2(_movementValues[_currentValueIndex].playerSpeed, _rigidBody.velocity.y);
    }
}
