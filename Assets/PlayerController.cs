using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float maxPlayerSpeed = 125.0f;
    [SerializeField] private float playerSpeed = 0.0f;
    [SerializeField] private float acceleration = 25.0f;
    [SerializeField] private float jumpPower = 5.0f;

    private float deceleration = 0.0f;
    private Rigidbody2D _rigidBody;

    private bool _goingLeft = false;

    private float _magnitudeForFloorCollision = 0.7f;
    private bool _canJump;

    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        if (_rigidBody == null)
            Debug.LogError("please add a Rigidbody2D component");
        deceleration = acceleration * 5;
        _animator = GetComponent<Animator>();
    }

    bool canJump() {
        const int playerMask = (1 << 3);
        int everythingExpectPlayerMask = ~playerMask;
        _canJump = Physics2D.Raycast(transform.position, Vector2.down, _magnitudeForFloorCollision, everythingExpectPlayerMask);
        Debug.DrawRay(transform.position, Vector2.down * _magnitudeForFloorCollision, Color.red);
        return _canJump;
    }

    void FlipPlayerHorizontally() {
        var tempScale = transform.localScale;
        tempScale.x *= -1;
        transform.localScale = tempScale;
        _goingLeft = !_goingLeft;
    }

    void Animate(bool idle) {
        if (_rigidBody.velocity.y > 0  && !_canJump) {
            _animator.Play("Jump");
        } else if (_rigidBody.velocity.y < 0 && !_canJump) {
            _animator.Play("Fall");
        } else if (idle) {
            _animator.Play("Idle");
        } else
            _animator.Play("Walking");
    }

    void Move()
    {
        float input = Input.GetAxisRaw("Horizontal");
        bool oppositeDirection = _goingLeft && input < 0 || !_goingLeft && input > 0;
        bool oppositeVelocity = _goingLeft && _rigidBody.velocity.x < 0 || !_goingLeft && _rigidBody.velocity.x > 0;

        if (oppositeDirection) {
            FlipPlayerHorizontally();
        }

        if ((input > 0 && playerSpeed < maxPlayerSpeed) || (input < 0 && playerSpeed > -maxPlayerSpeed)) {
            playerSpeed += (oppositeVelocity ? deceleration : acceleration) * input * Time.deltaTime;
        } else {
            if(playerSpeed > deceleration * Time.deltaTime)
                playerSpeed = playerSpeed - deceleration * Time.deltaTime;
            else if(playerSpeed < -deceleration * Time.deltaTime)
                playerSpeed = playerSpeed + deceleration * Time.deltaTime;
            else {
                playerSpeed = 0;
            }
        }

        Animate(input == 0);

        _rigidBody.velocity = new Vector2(playerSpeed * Time.deltaTime, _rigidBody.velocity.y);
    }
    private void Jump() {
        _rigidBody.velocity = new Vector2(0, jumpPower);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        Move();

        if (canJump() && Input.GetButtonDown("Jump"))
            Jump();
    }
}
