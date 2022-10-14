using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testMove : MonoBehaviour
{
    public Transform moveWith;


    void Update()
    {
        transform.position = new Vector3(moveWith.position.x, moveWith.position.y + 2, transform.position.z);
    }
}
