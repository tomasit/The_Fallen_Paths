using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] public float maxPlayerSpeed = 125.0f;
    [SerializeField] public float playerSpeed = 0.0f;
    [SerializeField] private float acceleration = 25.0f;
    [SerializeField] public float jumpPower = 5.0f;

    private float deceleration = 0.0f;

    private bool _goingLeft = false;

    private bool _isGrounded;

    private Rigidbody2D _rigidBody;

    private Animator _animator;

    private BoxCollider2D _collider;

    private int _everythingExpectPlayerLayerMask = 0;

    private const float _groundedRayMagnitude = 0.075f;

    // Start is called before the first frame update
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<BoxCollider2D>();
        deceleration = acceleration * 3.5f;
        _everythingExpectPlayerLayerMask = ~(1 << 7);
    }

    bool isGrounded()
    {
        Vector2 rayPosition = transform.position;

        rayPosition.x = rayPosition.x - (_collider.size.x * transform.localScale.x) / 2;
        rayPosition.y = rayPosition.y - (_collider.size.y * transform.localScale.y) / 2 - _groundedRayMagnitude;

        RaycastHit2D hit = Physics2D.Raycast(rayPosition, Vector2.right, _collider.size.x * transform.localScale.x, _everythingExpectPlayerLayerMask);
        _isGrounded = hit;

        Debug.DrawRay(rayPosition, Vector2.right * _collider.size.x * transform.localScale.x, _isGrounded ? Color.red : Color.green);
        // Adapt position
    //    PositionHotfix(rayPosition, hit);
        return _isGrounded;
    }

    void FlipPlayerHorizontally()
    {
        var tempScale = transform.localScale;
        tempScale.x *= -1;
        transform.localScale = tempScale;
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

        if ((input > 0 && playerSpeed < maxPlayerSpeed) || (input < 0 && playerSpeed > -maxPlayerSpeed))
        {
            if (oppositeDirection)
                playerSpeed = -playerSpeed * 0.8f;
            else
                playerSpeed += acceleration * input * Time.deltaTime;
        }
        else
        {
            if (playerSpeed > deceleration * Time.deltaTime)
                playerSpeed = playerSpeed - deceleration * Time.deltaTime;
            else if (playerSpeed < -deceleration * Time.deltaTime)
                playerSpeed = playerSpeed + deceleration * Time.deltaTime;
            else
                playerSpeed = 0;
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
        var verticalHit = Physics2D.Raycast(safeRayPosition, Vector2.down, halfColliderHeight + _groundedRayMagnitude, _everythingExpectPlayerLayerMask);

        if (!verticalHit)
            return;
        var fixedPosition = transform.position;
        fixedPosition.y -= (transform.position.y - (halfColliderHeight) - verticalHit.point.y);
        transform.position = fixedPosition;
        Debug.DrawRay(safeRayPosition, Vector2.down * (halfColliderHeight + _groundedRayMagnitude), _isGrounded ? Color.red : Color.green);
    }

    private void Jump()
    {
        _rigidBody.velocity = new Vector2(0, jumpPower);
    }
    // Update is called once per frame
    void Update()
    {
        if (_animator.GetBool("Dead"))
            playerSpeed = 0;
        else
        {
            if (isGrounded() && Input.GetButtonDown("Jump"))
                Jump();

            float input = Input.GetAxisRaw("Horizontal");

            if (_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "PlayerHit")
                Move(input);
            AnimateMovement(input == 0);

            // NOTE: For test purposes
            if (Input.GetButtonDown("Debug Fire"))
            {
                GetComponent<BasicHealthWrapper>().Hit(1);
                playerSpeed = 0;
            }
        }
    }

    private void FixedUpdate()
    {
        _rigidBody.velocity = new Vector2(playerSpeed, _rigidBody.velocity.y);
    }
}
