using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class TestRangedPower : AVisualCircleRangedPower
{
    private GameObject preview = null;
    [SerializeField] private GameObject previewPrefab = null;

    private Vector2 _calculatedPosition;

    private BoxCollider2D _collider = null;
    private void Start()
    {
        _collider = GetComponent<BoxCollider2D>();
    }

    public override void Fire()
    {
        // var newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // newPosition.z = 0;
        // transform.position = preview.transform.position;
        firingPower = false;
    }

    protected override void Preview()
    {
        if (previewPrefab == null)
            return;
        if (preview == null)
        {
            preview = Instantiate(previewPrefab, transform);
            preview.transform.parent = transform.parent;
            preview.transform.localScale = transform.localScale;
        }
        Vector2 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Debug.Log(transform.localScale);

        var yColliderOffset = new Vector2(0, (_collider.size.y * transform.localScale.y) / 2);

        var ray = Physics2D.Raycast(newPosition - yColliderOffset, Vector3.down, 1.5f, (1 << 11));

        if (ray)
        {
            preview.transform.position = (ray.point + yColliderOffset);
            Debug.DrawLine(newPosition - yColliderOffset, ray.point, Color.red);
            // var box = Physics2D.BoxCast(preview.transform.position, _collider.size, 0, Vector2.zero, 1, (1 << 11));
        }
        var box = BoxCastDrawer.BoxCastAndDraw(preview.transform.position, _collider.size * transform.localScale, 0, Vector2.zero, -0.1f, (1 << 11));

        if (box)
        {
            preview.GetComponent<SpriteRenderer>().color = Color.red;
            preview.transform.position = newPosition;
        }
        else
            preview.GetComponent<SpriteRenderer>().color = Color.blue;

        // else
        //     preview.transform.position = newPosition;
    }

    protected override void UnPreview()
    {
        if (preview != null)
        {
            Destroy(preview.gameObject);
            preview = null;
        }
    }


    protected override bool canCastPower()
    {
        return true;
    }
}
