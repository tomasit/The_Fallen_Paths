using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static EnemyInfo;

public class EnemyEventsManager : MonoBehaviour
{
    public Enemy[] Enemies;
    public LayerMask[] layersToIgnore;
    private Transform player;
    private AnimatorStateMachine animatorController;

    void Start()
    {
        player = ((PlayerController)FindObjectOfType(typeof(PlayerController))).transform;
        animatorController = GetComponent<AnimatorStateMachine>();

        for (int i = 0; i < Enemies.Length; ++i)
        {
            IgnoreLayers(Enemies[i]);
            InitEnemyComponents(Enemies[i]);
            if (Enemies[i].roomProprieties != null)
                Enemies[i].movementManager.target = Enemies[i].roomProprieties.targets[0];
            Enemies[i].healtWrapper.SetAnimator(Enemies[i].animator);
            Enemies[i].healtWrapper.SetMaxHealth(EnemyInfo.Health[Enemies[i].type]);
            Enemies[i].detectionManager.SetDetectionDistance(DetectionDistance[Enemies[i].type]);
        }
    }

    public void LinkEnemies(int sourceInstanceID, int destInstanceID)
    {
        Enemy source = null;
        Enemy dest = null;
        foreach (var enemy in Enemies)
            if (enemy.entity.GetInstanceID() == sourceInstanceID)
                source = enemy;
            else if (enemy.entity.GetInstanceID() == destInstanceID)
                dest = enemy;
        dest.entity.GetComponent<EnemyDetectionManager>()._sourceBehavior = source.entity;
    }
    public void UnlinkEnemies(int destInstanceID)
    {
        Enemy dest = null;
        foreach (var enemy in Enemies)
            if (enemy.entity.GetInstanceID() == destInstanceID)
                dest = enemy;
        dest.entity.GetComponent<EnemyDetectionManager>()._sourceBehavior = null;
    }

    private void InitEnemyComponents(Enemy enemy)
    {
        enemy.uuid = System.Guid.NewGuid().ToString();
        enemy.sprite = enemy.entity.transform.GetChild(0).GetComponent<SpriteRenderer>();
        enemy.animator = enemy.entity.transform.GetChild(0).GetComponent<Animator>();
        enemy.dialogs = enemy.entity.GetComponent<TMPDialogue>();
        enemy.agentMovement = enemy.entity.GetComponent<Agent>();
        enemy.movementManager = enemy.entity.GetComponent<AEnemyMovement>();
        enemy.detectionManager = enemy.entity.GetComponent<EnemyDetectionManager>();
        enemy.healtWrapper = enemy.entity.GetComponent<BasicHealthWrapper>();
        enemy.dialogManager = enemy.entity.GetComponent<EnemyDialogManager>();
    }

    void Update()
    {
        foreach (var enemy in Enemies)
        {
            if (!enemy.enabled)
            {
                continue;
            }
            if (enemy.detectionManager.GetState() == DetectionState.None && enemy.roomProprieties != null)
            {
                RoomTargetPoints(enemy);
                enemy.movementManager.target = enemy.roomProprieties.targets[enemy.roomProprieties.targetIndex];
            }
            if (enemy.detectionManager.GetState() == DetectionState.Flee && enemy.fleePoints != null)
            {
                FleeTargetPoints(enemy);
                enemy.movementManager.target = enemy.fleePoints.targets[enemy.fleePoints.targetIndex];
            }
            RotateEnemies(enemy);
            DetectionEventState(enemy);
            AnimationStateMachine(enemy);
            CheckResetState(enemy);
        }
    }

    private void CheckResetState(Enemy enemy)
    {
        if (enemy.healtWrapper.isDead())
        {
            enemy.enabled = false;

            enemy.detectionManager.Enable(false);
            enemy.detectionManager.SetState(DetectionState.None, false);

            enemy.movementManager.target = null;
            enemy.movementManager.isAtDistanceToInteract = false;
            enemy.movementManager.speed = 0;
            enemy.movementManager.collisionObj = null;
            enemy.movementManager.isClimbing = false;
            enemy.movementManager.isEndClimbing = false;

            enemy.agentMovement.DisableAgent();

            enemy.entity.GetComponent<CoroutineProcessor>().DisableTriggerInteractor(true);
            enemy.entity.GetComponent<CoroutineProcessor>().Enable(false);

            enemy.dialogManager.Enable(false);

            enemy.entity.GetComponent<Collider2D>().enabled = false;

        }
    }

    private void IgnoreLayers(Enemy enemy)
    {
        foreach (LayerMask layer in layersToIgnore)
        {
            // NOTE : if the layerMask is the 31, it didn't work
            int layerValue = (int)Mathf.Log(layer.value, 2);
            Physics2D.IgnoreLayerCollision(layerValue, enemy.entity.layer, true);
        }
    }

    private void FleeTargetPoints(Enemy enemy)
    {
        RoomProprieties fleePoints = enemy.fleePoints;
        AEnemyMovement movManager = enemy.movementManager;

        int index = 0;
        var playerDirection = FindTargetDirection(movManager.gameObject.transform.position, player.position);

        foreach (var point in fleePoints.targets)
        {
            var pointDirection = FindTargetDirection(point.position, movManager.gameObject.transform.position);

            if (playerDirection.x < 0 && pointDirection.x < 0)
            {
                fleePoints.targetIndex = index;
                return;
            }
            else if (playerDirection.x > 0 && pointDirection.x > 0)
            {
                fleePoints.targetIndex = index;
                return;
            }

            if (RangeOf(pointDirection.x, playerDirection.x, 0.001f))
            {
                if (index + 1 > fleePoints.targets.Length - 1)
                    index = 0;
                else
                    index++;
                fleePoints.targetIndex = index;
                return;
            }
            ++index;
        }
    }

    private void RoomTargetPoints(Enemy enemy)
    {
        RoomProprieties room = enemy.roomProprieties;
        AEnemyMovement movManager = enemy.movementManager;

        if (RangeOf(movManager.transform.position.x, room.targets[room.targetIndex].position.x, 0.1f) &&
            RangeOf(movManager.transform.position.y, room.targets[room.targetIndex].position.y, 0.75f))
        {
            if ((room.targets.Length - 1) == room.targetIndex)
                room.targetIndex = 0;
            else
                room.targetIndex += 1;
        }
    }

    private void RotateEnemies(Enemy enemy)
    {
        if (enemy.movementManager.HasMovedFromLastFrame())
        {
            if (enemy.type != EnemyType.Archer && enemy.type != EnemyType.Mage)
            {
                if (enemy.detectionManager.playerDetected)
                {
                    return;
                }
            }
            if (enemy.movementManager.DirectionMovedFromLastFrame() < 0)
            {
                enemy.entity.transform.eulerAngles = new Vector3(0, 180, 0);
            }
            else
            {
                enemy.entity.transform.eulerAngles = new Vector3(0, 0, 0);
            }
        }
        else
        {
            if (enemy.detectionManager.direction.x < 0)
            {
                enemy.entity.transform.eulerAngles = new Vector3(0, 180, 0);
            }
            else if (enemy.detectionManager.direction.x > 0)
            {
                enemy.entity.transform.eulerAngles = new Vector3(0, 0, 0);
            }
        }
    }

    private void DetectionEventState(Enemy enemy)
    {
        var dtcManager = enemy.detectionManager;
        var movManager = enemy.movementManager;
        var playerHealthWrp = player.GetComponent<BasicHealthWrapper>();

        if (dtcManager._sourceBehavior != null)
            dtcManager.SetState(dtcManager._sourceBehavior.GetComponent<EnemyDetectionManager>().GetState());

        if (dtcManager.GetState() == DetectionState.None || playerHealthWrp.isDead())
        {
            movManager.BasicMovement();
        }
        else if (dtcManager.GetState() == DetectionState.Alert)
        {
            movManager.AlertMovement();
            if (dtcManager._sourceBehavior != null)
                dtcManager._sourceBehavior.GetComponent<AEnemyMovement>().target = movManager.target;
        }
        else if (dtcManager.GetState() == DetectionState.Spoted)
        {
            movManager.SpotMovement();
            if (dtcManager._sourceBehavior != null)
                dtcManager._sourceBehavior.GetComponent<AEnemyMovement>().target = movManager.target;
        }
        else if (dtcManager.GetState() == DetectionState.Flee)
        {
            movManager.FleeMovement();
        }
        else if (dtcManager.GetState() == DetectionState.Freeze)
        {
            movManager.FreezeMovement();
        }
    }

    private void AnimationStateMachine(Enemy enemy)
    {
        Vector3 targetDistance = FindTargetDirection(
            enemy.movementManager.spritePos.position,
            enemy.movementManager.target.position);

        bool isAtTargetPosition = false;
        bool isClimbing = enemy.movementManager.isEndClimbing || enemy.movementManager.isClimbing;

        if (targetDistance.x >= 0)
        {
            if (targetDistance.x < 0.1f && RangeOf(targetDistance.y, 0f, 0.80f))
            {
                isAtTargetPosition = true;
            }
        }
        else if (targetDistance.x <= 0)
        {
            if (targetDistance.x > -0.1f && RangeOf(targetDistance.y, 0f, 0.80f))
            {
                isAtTargetPosition = true;
            }
        }

        animatorController.Idle(enemy, isAtTargetPosition, isClimbing);
        animatorController.Scared(enemy, isAtTargetPosition, isClimbing);
        animatorController.Moving(enemy, isAtTargetPosition, isClimbing);
        animatorController.Fight(enemy, isClimbing);
        animatorController.Climbing(enemy, targetDistance, isAtTargetPosition, isClimbing);
    }
}
