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

    
    private bool _canMove;

    // Start is called before the first frame update
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        if (_rigidBody == null)
            Debug.LogError("please add a Rigidbody2D component");    
        deceleration = acceleration * 5;
    }

    bool canJump() => true;


    void Move()
    {
        float input = Input.GetAxisRaw("Horizontal");

        if ((input > 0 && playerSpeed < maxPlayerSpeed) || (input < 0 && playerSpeed > -maxPlayerSpeed)  )
            playerSpeed += (acceleration * input) * Time.deltaTime;
        else {
            if(playerSpeed > deceleration * Time.deltaTime)
                playerSpeed = playerSpeed - deceleration * Time.deltaTime;
            else if(playerSpeed < -deceleration * Time.deltaTime)
                playerSpeed = playerSpeed + deceleration * Time.deltaTime;
            else
                playerSpeed = 0;
        }
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
