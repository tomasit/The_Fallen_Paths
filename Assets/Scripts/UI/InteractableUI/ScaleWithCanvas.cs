using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleWithCanvas : MonoBehaviour
{
    [SerializeField] private Transform _canvas;

    private void Update()
    {
        GetComponent<RectTransform>().sizeDelta = new Vector2(_canvas.GetComponent<RectTransform>().rect.width, _canvas.GetComponent<RectTransform>().rect.height);
    }
}
