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
    
    private bool _canMove;

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
        return Physics2D.Raycast(transform.position, Vector2.down, 0.7f, everythingExpectPlayerMask);
    }

    void FlipSprite() {
        var tempScale = transform.localScale;
        tempScale.x *= -1;
        transform.localScale = tempScale;
        _goingLeft = !_goingLeft;
    }

    void Move()
    {
        float input = Input.GetAxisRaw("Horizontal");
        bool oppositeDirection = _goingLeft && input < 0 || !_goingLeft && input > 0;
        bool oppositeVelocity = _goingLeft && _rigidBody.velocity.x < 0 || !_goingLeft && _rigidBody.velocity.x > 0;
        
        if (oppositeDirection) {
            FlipSprite();
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
        if (input == 0) {
            _animator.Play("Idle");
        } else 
            _animator.Play("Walking");
        
        _rigidBody.velocity = new Vector2(playerSpeed * Time.deltaTime, _rigidBody.velocity.y);
    }
    private void Jump() => _rigidBody.velocity = new Vector2(0, jumpPower);
    // Update is called once per frame
    void FixedUpdate()
    {
        Move();

        if (Input.GetButtonDown("Jump") && canJump())
            Jump();
    }
}
