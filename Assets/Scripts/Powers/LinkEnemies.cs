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

    private EnemyEventsManager _enemyEventManager = null;

    private enum EnemyIndex
    {
        FIRST_ENEMY,
        SECOND_ENEMY
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        _enemiesPreview = new ParticleSystem[2];
    }

    public override void Cancel()
    {
        _sourceEnemy = null;
        _destEnemy = null;
    }

    public override void Fire()
    {
        float _powerMaxDuration = _powerManager.GetPowerMaxDurationFromStackTrace(1);
    }

    protected override void Preview()
    {
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (_mousePreview == null)
        {
            _mousePreview = Instantiate(_enemyPreviewPrefab, mousePosition, Quaternion.identity);
        }
        _mousePreview.transform.position = mousePosition;
    }

    protected override void UnPreview()
    {
        if (_mousePreview != null)
        {
            Destroy(_mousePreview.gameObject);
            _mousePreview = null;
            Debug.Log("???????????");
        }
    }

    protected override bool canCastPower()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, float.PositiveInfinity, (1 << LayerMask.NameToLayer("Enemy")));

        if (hit == false)
            return false;

        Debug.Log(hit.collider.gameObject.name + " " + Input.GetMouseButton(0));
        if (_sourceEnemy == null && Input.GetMouseButton(0))
        {
            _sourceEnemy = hit.collider.gameObject;
            if (_enemyEventManager == null)
                _enemyEventManager = _sourceEnemy.transform.GetComponentInParent<EnemyEventsManager>();
            _enemiesPreview[0] = Instantiate(_enemyPreviewPrefab, _sourceEnemy.transform.position, Quaternion.identity);
            _enemiesPreview[0].transform.parent = _sourceEnemy.transform;
            _enemiesPreview[0].transform.localScale = new Vector3(1, 1, 1);
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

            if (sourceType != null && sourceType == destType && Input.GetMouseButton(0))
            {
                _destEnemy = supposedDestEnemy;
                _enemiesPreview[1] = Instantiate(_enemyPreviewPrefab, _destEnemy.transform.position, Quaternion.identity);
                _enemiesPreview[1].transform.parent = _destEnemy.transform;
                _enemiesPreview[1].transform.localScale = new Vector3(1, 1, 1);
                return true;
            }
            return false;
        }
        return false;
    }
}
