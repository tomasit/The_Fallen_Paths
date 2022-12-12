using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;

public class SubLevelChange : AInteractable
{
    public GameObject _player;
    public Tilemap _grid;
    public bool _interactOnStart = false;
    public bool _translateChange = false;
    public float _translateDuration = 2.0f;
    public bool _fadeChange = false;
    public string _quoteName = "";
    public SoundData.SoundEffectName _soundType;
    private Camera _camera = null;
    private bool _isTransitionning = false;
    private SoundEffect _soundEffectPlayer;
    public TransitionScreen _transitionScreen;
    public Transform[] _translateMoveTo;
    public Transform _fadeTpPosition;
    public bool _teleportPlayer = false;
    public Transform _translateTpPosition;

    private void Start()
    {
        _soundEffectPlayer = GetComponent<SoundEffect>();
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

    private void BlockInput(bool block)
    {
        PlayerController ctrl = _player.GetComponent<PlayerController>();
        ComputeInteraction itrct = _player.GetComponent<ComputeInteraction>();

        if (ctrl)
            ctrl.BlockInput(block);
        if (itrct)
            itrct.BlockInput(block);
    }

    private Vector2 GetDistance(int i)
    {
        Vector2 dist;
        dist.x = _translateMoveTo[i].position.x - _player.transform.position.x;
        dist.y = _translateMoveTo[i].position.y - _player.transform.position.y;
        return dist;
    }

    private Vector2 GetDirection(Vector2 dist)
    {
        Vector2 dir;

        if (Mathf.Abs(dist.x) >= Mathf.Abs(dist.y))
        {
            dir.x = (dist.x > 0 ? 1 : -1);
            dir.y = 0;
        }
        else
        {
            dir.x = 0;
            dir.y = (dist.y > 0 ? 1 : -1);
        }
        return dir;
    }

    private IEnumerator MovePlayerToPositions()
    {
        int i = 0;

        var dist = GetDistance(i);
        var dir = GetDirection(dist);
        // if (dir.y != 0)
        //     _player.GetComponent<PlayerController>().ActiveLadder(true);
        // else
        //     _player.GetComponent<PlayerController>().ActiveLadder(false);

        float distance = 0.0f;

        float oldP = (dir.x == 0 ? _player.transform.position.y : _player.transform.position.x);

        while (i < _translateMoveTo.Length)
        {
            // if (dir.x == 0)
            // {
            //     _player.GetComponent<PlayerController>().LadderMovement(dir.y, dir.x);
            //     Debug.Log("direction x: " + dir.x + " direction y = " + dir.y);
            //     if (oldP != _player.transform.position.y)
            //     {
            //         distance += _player.transform.position.y - oldP;
            //         oldP = _player.transform.position.y;
            //     }
            // }
            // else
            // {
            _player.GetComponent<PlayerController>().Move(dir.x);
            _player.GetComponent<PlayerController>().AnimateMovement(false);
            if (oldP != _player.transform.position.x)
            {
                distance += _player.transform.position.x - oldP;
                oldP = _player.transform.position.x;
            }
            // }

            //Debug.Log("distance = " + distance);

            if (Mathf.Abs(distance) >= (dir.x == 0 ? Mathf.Abs(dist.y) : Mathf.Abs(dist.x)))
            {
                distance = 0.0f;
                i++;
                if (i < _translateMoveTo.Length)
                {
                    _player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                    dist = GetDistance(i);
                    dir = GetDirection(dist);
                    if (dir.y != 0)
                        _player.GetComponent<PlayerController>().ActiveLadder(true);
                    else
                        _player.GetComponent<PlayerController>().ActiveLadder(false);
                    Debug.Log("Distance x = " + dist.x + " Distance y = " + dist.y);
                    Debug.Log("direction x = " + dir.x + " direction y = " + dir.y);
                    oldP = (dir.x == 0 ? _player.transform.position.y : _player.transform.position.x);
                }
            }
            yield return null;
        }
    }

    private IEnumerator TranslateTo(Tilemap grid)
    {
        BlockInput(true);
        _isTransitionning = true;

        if (_teleportPlayer)
            _player.transform.position = _translateTpPosition.position;

        Vector3 target = _grid.transform.position + _grid.cellBounds.center;
        target.z = _camera.transform.position.z;
        float newOrthoSize = ((grid.cellBounds.size.x > grid.cellBounds.size.y * _camera.aspect) ? (float)grid.cellBounds.size.x / (float)_camera.pixelWidth * _camera.pixelHeight : grid.cellBounds.size.y) / 2;
        float oldOrthoSize = _camera.orthographicSize;
        Vector3 cameraOldPos = _camera.transform.position;

        for (float t = 0.0f; _camera.transform.position != target || _camera.orthographicSize != newOrthoSize; t += Time.deltaTime / _translateDuration)
        {
            _camera.transform.position = Vector3.Lerp(cameraOldPos, target, t);
            _camera.orthographicSize = Mathf.Lerp(oldOrthoSize, newOrthoSize, t);
            yield return null;
        }

        if (_translateMoveTo.Length > 0)
            yield return StartCoroutine(MovePlayerToPositions());

        _isTransitionning = false;
        BlockInput(false);
    }

    private IEnumerator Fade()
    {
        _isTransitionning = true;
        BlockInput(true);
        _transitionScreen.StartTransition(_quoteName);
        while (_transitionScreen.GetTransitionState() != TransitionScreen.TransitionState.MIDDLE)
            yield return null;
        _player.transform.position = _fadeTpPosition.position;
        MoveAndResizeCamera();
        while (_transitionScreen.GetTransitionState() != TransitionScreen.TransitionState.NONE)
            yield return null;
        BlockInput(false);
        _isTransitionning = false;
    }

    public bool IsInTransition()
    {
        return _isTransitionning;
    }

    public override void Interact()
    {
        if (_soundEffectPlayer != null)
            _soundEffectPlayer.PlaySound(_soundType);
        FitWithWorldSize camResize = _camera.GetComponent<FitWithWorldSize>();

        if (camResize != null)
        {
            if (_translateChange)
            {
                Vector3Int bounds = _grid.cellBounds.size;
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
