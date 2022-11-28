using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkEnemies : AVisualCircleRangedPower
{
    private GameObject _sourceEnemy = null;
    private GameObject _destEnemy = null;

    [SerializeField] private ParticleSystem _enemyPreviewPrefab = null;
    private ParticleSystem _mousePreview = null;
    private ParticleSystem[] _enemiesPreview = null;

    private LineRenderer _linePreview = null;
    [SerializeField] private LineRenderer _linePreviewPrefab = null;

    private EnemyEventsManager _enemyEventManager = null;

    private float _powerMaxDuration;
    private float _powerTimer = 0;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        _powerMaxDuration = _powerManager.GetPowerMaxDurationFromStackTrace(1);
        _enemiesPreview = new ParticleSystem[2];
    }

    public override void Cancel()
    {
        firingPower = false;
        _sourceEnemy = null;
        _destEnemy = null;
        UnPreview();
        foreach (var preview in _enemiesPreview)
            if (preview != null)
                Destroy(preview.gameObject);
        _enemiesPreview[0] = null;
        _enemiesPreview[1] = null;
    }

    public override void Fire()
    {
        _powerTimer += Time.deltaTime;
        if (_powerTimer >= _powerMaxDuration)
        {
            _powerTimer = 0;
            Cancel();
            return;
        }
        _linePreview.SetPosition(0, _sourceEnemy.GetComponentInChildren<SpriteRenderer>().transform.position);
        _linePreview.SetPosition(1, _destEnemy.GetComponentInChildren<SpriteRenderer>().transform.position);
        _enemiesPreview[0].transform.position = _sourceEnemy.transform.position;
    }

    protected override void Preview()
    {
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        if (_mousePreview == null)
        {
            _mousePreview = Instantiate(_enemyPreviewPrefab, mousePosition, Quaternion.identity);
        }
        _mousePreview.transform.position = mousePosition;

        if (_sourceEnemy != null)
        {
            if (_linePreview == null)
                _linePreview = Instantiate(_linePreviewPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            _linePreview.SetPosition(0, _sourceEnemy.GetComponentInChildren<SpriteRenderer>().transform.position);
            _linePreview.SetPosition(1, mousePosition);
        }
    }

    protected override void UnPreview()
    {
        if (_mousePreview != null && firingPower == false)
        {
            Destroy(_mousePreview.gameObject);
            _mousePreview = null;
        }
        if (_linePreview != null && firingPower == false)
        {
            var callerPowerType = new System.Diagnostics.StackTrace().GetFrame(1).GetMethod();
            Destroy(_linePreview.gameObject);
            _linePreview = null;
        }
    }

    private void InstantiateEnemyPreview(ref ParticleSystem preview, Transform enemyTransform)
    {
        preview = Instantiate(_enemyPreviewPrefab, enemyTransform.position, Quaternion.identity);
        preview.transform.parent = enemyTransform;
        preview.transform.localScale = new Vector3(1, 1, 1);
    }

    protected override bool canCastPower()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, float.PositiveInfinity, (1 << LayerMask.NameToLayer("Enemy")));

        if (hit == false)
            return false;

        if (_sourceEnemy == null && Input.GetMouseButtonDown(0))
        {
            _sourceEnemy = hit.collider.gameObject;
            if (_enemyEventManager == null)
                _enemyEventManager = _sourceEnemy.transform.GetComponentInParent<EnemyEventsManager>();
            InstantiateEnemyPreview(ref _enemiesPreview[0], _sourceEnemy.transform);
            _linePreview = Instantiate(_linePreviewPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            return false;
        }
        else if (_sourceEnemy != null && _sourceEnemy.gameObject.GetInstanceID() != hit.collider.gameObject.GetInstanceID())
        {
            var supposedDestEnemy = hit.collider.gameObject;
            EnemyType? sourceType = null;
            EnemyType? destType = null;

            foreach (var enemy in _enemyEventManager.Enemies)
                if (enemy.entity.GetInstanceID() == _sourceEnemy.GetInstanceID())
                    sourceType = enemy.type;
                else if (enemy.entity.GetInstanceID() == supposedDestEnemy.GetInstanceID())
                    destType = enemy.type;

            if (sourceType != null && sourceType == destType && Input.GetMouseButtonDown(0))
            {
                _powerManager.ActivatePowerCooldownFromStackTrace();

                _destEnemy = supposedDestEnemy;
                InstantiateEnemyPreview(ref _enemiesPreview[1], _destEnemy.transform);
                return true;
            }
            return false;
        }
        return false;
    }
}
