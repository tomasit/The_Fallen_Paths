using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractionDisplayText : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private RectTransform _canvasRect;
    [SerializeField] private TextMeshProUGUI _tmproUGUI = null;
    private GameObject _target;
    private Vector3 _offset;
    private RectTransform _selfRect;

    private void Awake()
    {
        _selfRect = GetComponent<RectTransform>();
    }

    public void SetText(string text)
    {
        if (_tmproUGUI != null)
            _tmproUGUI.text = text;
    }

    public void SetLayout(GameObject target, Vector3 offset, bool active)
    {
        _target = target;
        _offset = offset;
        transform.gameObject.SetActive(active);
    }

    private void Update()
    {
        if (_target != null)
        {
            Vector2 viewportPosition = _camera.WorldToViewportPoint(_target.transform.position + _offset);
            Vector2 worldObject_ScreenPosition = new Vector2(
            ((viewportPosition.x * _canvasRect.sizeDelta.x) - (_canvasRect.sizeDelta.x * 0.5f)),
            ((viewportPosition.y * _canvasRect.sizeDelta.y) - (_canvasRect.sizeDelta.y * 0.5f)));

            _selfRect.anchoredPosition = worldObject_ScreenPosition;
        }
    }
}
