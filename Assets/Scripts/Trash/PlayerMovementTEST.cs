using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementTEST : MonoBehaviour
{
    public float speed = 10f;
    
    void Start()
    {
        
    }

    void Update()
    {
        AllowedMovement();

        ManageInputs();
    }

    private void ManageInputs()
    {
        float inputX = Input.GetAxis("Horizontal");

        Vector3 movement = new Vector3(speed * inputX, 0, 0);

        movement *= Time.deltaTime;

        transform.Translate(movement);

        if (Input.GetKeyDown(KeyCode.P)) {
            Debug.Log("Position x : " + transform.position.x);
        }
    }

    private void AllowedMovement() 
    {
        //prevent enemy from falling
        gameObject.GetComponent<Rigidbody2D>().freezeRotation = true;
    }
}
