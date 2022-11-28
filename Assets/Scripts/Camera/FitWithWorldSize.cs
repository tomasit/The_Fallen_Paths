using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FitWithWorldSize : MonoBehaviour
{
    public void FitLevelSize(float width, float height)
    {
        GetComponent<Camera>().orthographicSize = ((width > height * GetComponent<Camera>().aspect) ? (float)width / (float)GetComponent<Camera>().pixelWidth * GetComponent<Camera>().pixelHeight : height) / 2;
    }

    public void SetPosition(Vector2 position)
    {
        transform.position = new Vector3(position.x, position.y, transform.position.z);
    }
}
