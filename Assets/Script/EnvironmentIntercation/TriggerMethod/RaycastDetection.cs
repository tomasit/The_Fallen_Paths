using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastDetection : TriggerProcessor
{
    private enum Direction
    {
        LEFT,
        RIGHT,
        UP,
        DOWN,
    }

    [SerializeField] private Vector2 _offset;
    [SerializeField] private Direction _rayDirection;
    [SerializeField] private LayerMask _triggerLayer;
    [SerializeField] private float _rayLenght;
    private RaycastHit2D _hit;
    private Vector2 _vecDirection;

    private void Start()
    {
        _vecDirection = (_rayDirection == Direction.LEFT ? Vector2.left : (_rayDirection == Direction.RIGHT ? Vector2.right : (_rayDirection == Direction.UP ? Vector2.up : Vector2.down)));
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position + (Vector3)_offset, transform.position + (Vector3)_offset + (Vector3)(_rayDirection == Direction.LEFT ? Vector2.left : (_rayDirection == Direction.RIGHT ? Vector2.right : (_rayDirection == Direction.UP ? Vector2.up : Vector2.down))) * _rayLenght);
    }

    private void Update()
    {
        if (!_isDisable)
        {
            _hit = Physics2D.Raycast(transform.position + (Vector3)_offset, _vecDirection, _rayLenght, _triggerLayer);
            if (_hit.collider != null)
            {
                Trigger();
            }
        }
    }
}

