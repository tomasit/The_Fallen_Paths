using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRayMouse : MonoBehaviour
{
    private Vector2 r;
    private RaycastHit2D hit;

    void Update()
    {
        r = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        hit = Physics2D.Raycast(r, Vector2.zero);

        if (hit)
        {
            // Debug.Log("Bite");
            if (hit.transform.GetComponent<GlowOnTouch>())
            {
                hit.transform.GetComponent<GlowOnTouch>().Trigger(true);
            }
        }
    }
}
