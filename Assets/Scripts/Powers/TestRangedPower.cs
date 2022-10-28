using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class TestRangedPower : AVisualCircleRangedPower
{
    private GameObject preview = null;
    [SerializeField] private GameObject previewPrefab = null;

    private Vector2 _calculatedPosition;

    [SerializeField] private ParticleSystem _teleportationEffectPrefab = null;
    private bool _canCast = false;
    private BoxCollider2D _collider = null;
    private void Start()
    {
        _collider = GetComponent<BoxCollider2D>();
    }

    public override void Fire()
    {
        transform.position = _calculatedPosition;
        var tempParticles = Instantiate(_teleportationEffectPrefab, _calculatedPosition, Quaternion.identity);
        var tempMain = tempParticles.main;
        tempMain.startColor = _rangeColor;
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
        var previewSpriteRenderer = preview.GetComponent<SpriteRenderer>();

        if (Mathf.Sign(preview.transform.localScale.x) != Mathf.Sign(transform.localScale.x))
        {
            var tempScale = preview.transform.localScale;
            tempScale.x *= -1;
            preview.transform.localScale = tempScale;
        }
        previewSpriteRenderer.sprite = GetComponent<SpriteRenderer>().sprite;
        if (_canCast)
            previewSpriteRenderer.material.color = Color.blue;
        else
            previewSpriteRenderer.material.color = Color.red;

        preview.transform.position = _calculatedPosition;
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
        Vector2 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Debug.Log(transform.localScale);

        var yColliderOffset = new Vector2(0, (_collider.size.y * transform.localScale.y) / 2);

        var ray = Physics2D.Raycast(newPosition - yColliderOffset, Vector3.down, 1.5f, (1 << 11));

        if (ray)
        {
            _calculatedPosition = (ray.point + yColliderOffset);
            Debug.DrawLine(newPosition - yColliderOffset, ray.point, Color.red);
        }
        else
            _calculatedPosition = newPosition;

        var _colliderTempSize = _collider.size * transform.localScale;
        _colliderTempSize.y -= 0.1f;
        var box = BoxCastDrawer.BoxCastAndDraw(_calculatedPosition, _colliderTempSize, 0, Vector2.zero, -0.1f, (1 << 11));

        _canCast = !box;
        if (!_canCast)
            _calculatedPosition = newPosition;

        return (_canCast);
    }
}
