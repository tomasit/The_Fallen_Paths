using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;

public class SubLevelChange : AInteractable
{
    public Tilemap _grid;
    public Image _fadeImage;
    public TextMeshProUGUI _tmproUGUI;
    public bool _interactOnStart = false;
    public bool _translateChange = false;
    public float _translateSpeed = 1.0f;
    public bool _fadeChange = false;
    public string _fadeDescription = "";
    public float _fadeDuration = 1.0f;
    public float _descriptionDuration = 1.0f;
    private Camera _camera = null;

    private void Start()
    {
        if (!_camera)
            _camera = Camera.main;
        if (_interactOnStart)
        {
            MoveAndResizeCamera();
        }
    }

    private void MoveAndResizeCamera()
    {
        FitWithWorldSize camResize = _camera.GetComponent<FitWithWorldSize>();
        Vector3Int bounds = _grid.cellBounds.size;
        camResize.FitLevelSize(bounds.x, bounds.y);
        camResize.SetPosition(_grid.transform.position + _grid.cellBounds.center);
    }

    private IEnumerator TranslateTo(Tilemap grid)
    {
        Vector3 target = _grid.transform.position + _grid.cellBounds.center;
        target.z = _camera.transform.position.z;

        while (_camera.transform.position != target)
        {
            _camera.transform.position = Vector3.MoveTowards(_camera.transform.position, target, _translateSpeed * Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    private IEnumerator BlackFade(bool fadeIn)
    {
        Color baseImgColor = _fadeImage.color;
        Color nextImgColor = new Color(baseImgColor.r, baseImgColor.g, baseImgColor.b, fadeIn ? 1.0f : 0.0f);
        Color baseTxtColor = _tmproUGUI.color;
        Color nextTxtColor = new Color(baseTxtColor.r, baseTxtColor.g, baseTxtColor.b, fadeIn ? 1.0f : 0.0f);

        _tmproUGUI.text = _fadeDescription;

        yield return new WaitForSeconds(Time.deltaTime);

        for (float t = 0; t < 1.0f; t += Time.deltaTime / _fadeDuration)
        {
            _fadeImage.color = Color.Lerp(baseImgColor, nextImgColor, t);
            _tmproUGUI.color = Color.Lerp(baseTxtColor, nextTxtColor, t);
            yield return null;
        }
    }

    private IEnumerator Fade()
    {
        yield return StartCoroutine(BlackFade(true));
        MoveAndResizeCamera();
        yield return new WaitForSeconds(_descriptionDuration);
        yield return StartCoroutine(BlackFade(false));
    }

    public override void Interact()
    {
        FitWithWorldSize camResize = _camera.GetComponent<FitWithWorldSize>();

        if (camResize != null)
        {
            if (_translateChange)
            {
                Vector3Int bounds = _grid.cellBounds.size;
                camResize.FitLevelSize(bounds.x, bounds.y);
                StartCoroutine(TranslateTo(_grid));
            }
            else if (_fadeChange)
            {
                StartCoroutine(Fade());
            }
        }
    }

    public override void Save()
    {
    }

    public override void Load()
    {
    }
}
