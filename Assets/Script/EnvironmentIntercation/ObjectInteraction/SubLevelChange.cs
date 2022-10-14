using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SubLevelChange : AInteractable
{
    // Uncomment this if you want camera resize based on 4 sprites renderer

    // [SerializeField] private GameObject _topObj;
    // [SerializeField] private GameObject _botObj;
    // [SerializeField] private GameObject _leftObj;
    // [SerializeField] private GameObject _rightObj;

    // private Vector2 GetBounds()
    // {
    //     Vector2 bounds;
    //     Vector2 leftPos = new Vector2(_leftObj.transform.position.x - _leftObj.GetComponent<SpriteRenderer>().bounds.size.x / 2, _leftObj.transform.position.y);
    //     Vector2 rightPos = new Vector2(_rightObj.transform.position.x + _rightObj.GetComponent<SpriteRenderer>().bounds.size.x / 2, _rightObj.transform.position.y);
    //     Vector2 topPos = new Vector2(_topObj.transform.position.x, _topObj.transform.position.y + _topObj.GetComponent<SpriteRenderer>().bounds.size.y / 2);
    //     Vector2 botPos = new Vector2(_botObj.transform.position.x, _botObj.transform.position.y - _botObj.GetComponent<SpriteRenderer>().bounds.size.y / 2);

    //     bounds.x = Vector2.Distance(leftPos, rightPos);
    //     bounds.y = Vector2.Distance(topPos, botPos);

    //     return bounds;
    // }

    // private Vector2 GetPosition()
    // {
    //     return new Vector2(((_leftObj.transform.position.x - _leftObj.GetComponent<SpriteRenderer>().bounds.size.x * 0.5f) + (_rightObj.transform.position.x + _rightObj.GetComponent<SpriteRenderer>().bounds.size.x * 0.5f)) * 0.5f,
    //         ((_botObj.transform.position.y - _botObj.GetComponent<SpriteRenderer>().bounds.size.y * 0.5f) + (_topObj.transform.position.y + _topObj.GetComponent<SpriteRenderer>().bounds.size.y * 0.5f)) * 0.5f);
    // }

    [SerializeField] private Tilemap _grid;
    private Camera _camera = null;

    private void Start()
    {
        if (!_camera)
            _camera = Camera.main;
        Interact();
    }


    public override void Interact()
    {
        FitWithWorldSize camResize = _camera.GetComponent<FitWithWorldSize>();

        if (camResize != null)
        {
            // Uncomment this to 
            // Vector2 bounds = GetBounds();
            // camResize.FitLevelSize(bounds.x, bounds.y);
            // camResize.SetPosition(GetPosition());
            Vector3Int bounds = _grid.cellBounds.size;
            camResize.FitLevelSize(bounds.x, bounds.y);
            Debug.Log(bounds);
            camResize.SetPosition(_grid.transform.position + _grid.cellBounds.center);
        }
    }

    public override void Save()
    {
    }

    public override void Load()
    {
    }
}
