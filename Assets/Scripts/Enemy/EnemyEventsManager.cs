using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

        foreach(var enemy in Enemies) {
            IgnoreLayers(enemy);
            InitEnemyComponents(enemy);
            if (enemy.roomProprieties != null)
                enemy.movementManager.target = enemy.roomProprieties.targets[0];
            enemy.healtWrapper.SetAnimator(enemy.animator);
            enemy.healtWrapper.SetMaxHealth(EnemyInfo.Health[enemy.type]);
        }
    }

    private void InitEnemyComponents(Enemy enemy)
    {
        enemy.uuid = Guid.NewGuid().ToString();
        enemy.sprite = enemy.entity.transform.GetChild(0).GetComponent<SpriteRenderer>();
        enemy.animator = enemy.entity.transform.GetChild(0).GetComponent<Animator>();
        enemy.dialogs = enemy.entity.transform.GetChild(0).GetComponent<TMPDialogue>();
        enemy.agentMovement = enemy.entity.GetComponent<Agent>();
        enemy.movementManager = enemy.entity.GetComponent<AEnemyMovement>();
        enemy.detectionManager = enemy.entity.GetComponent<EnemyDetectionManager>();
        enemy.healtWrapper = enemy.entity.GetComponent<BasicHealthWrapper>();
    }

    void Update()
    {
        foreach(var enemy in Enemies) {
            if (!enemy.enabled) {
                continue;   
            }
            if (enemy.detectionManager.detectionState == DetectionState.None && enemy.roomProprieties != null) {
                RoomTargetPoints(enemy);
                enemy.movementManager.target = enemy.roomProprieties.targets[enemy.roomProprieties.targetIndex];
            }
            if (enemy.detectionManager.detectionState == DetectionState.Flee && enemy.fleePoints != null) {
                FleeTargetPoints(enemy);
                enemy.movementManager.target = enemy.fleePoints.targets[enemy.fleePoints.targetIndex];
            }
            RaycastDirection(enemy);
            DetectionEventState(enemy);
            AnimationStateMachine(enemy);
            CheckResetState(enemy);

            if (Input.GetKeyDown(KeyCode.Y)) {

                /*enemy.dialogs.SetUpTarget(
                    enemy.sprite.transform, 
                    new Vector3(
                            Camera.main.ScreenToWorldPoint(
                            0, 
                            enemy.sprite.sprite.rect.height / 2, 
                            0)
                        )
                );*/

                enemy.dialogs.StartDialogue(0);
            }
        }
    }

    private void CheckResetState(Enemy enemy)
    {
        //comme le player a tjr un collider il va continuer a le tapper
        if (player.GetComponent<BasicHealthWrapper>().isDead()) {
            enemy.detectionManager.SetState(DetectionState.None);
        }
        if (enemy.healtWrapper.isDead()) {
            enemy.enabled = false;
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

    private void RaycastDirection(Enemy enemy)
    {
        if (enemy.movementManager.target == null) {
            //Debug.Log("MovementManager target is null (eventManager issue)");
            return;
        }

        Vector3 directionPoint = FindTargetDirection(enemy.entity.transform.position, enemy.movementManager.target.position);

        if (directionPoint.x > 0) {
            enemy.detectionManager.SetRayCastDirection(Vector2.right);
        } else {
            enemy.detectionManager.SetRayCastDirection(Vector2.left);
        }
    }

    private void FleeTargetPoints(Enemy enemy)
    {
        RoomProprieties fleePoints = enemy.fleePoints;
        AEnemyMovement movManager = enemy.movementManager;

        int index = 0;
        var playerDirection = FindTargetDirection(movManager.gameObject.transform.position, player.position);
        
        foreach (var point in fleePoints.targets) {
            var pointDirection = FindTargetDirection(point.position, movManager.gameObject.transform.position);

            //Debug.Log("------------------");
            //Debug.Log("playerDir : " + playerDirection.x);
            //Debug.Log("pointDir : " + pointDirection.x);
            if (playerDirection.x < 0 && pointDirection.x < 0) {
                //Debug.Log("Je vais au point a droite : " + fleePoints.targets[index].gameObject.name + " index : " + index);
                fleePoints.targetIndex = index;
                return;
            } else if (playerDirection.x > 0 && pointDirection.x > 0) {
                //Debug.Log("Je vais au point a gauche : " + fleePoints.targets[index].gameObject.name + " index : " + index);
                fleePoints.targetIndex = index;
                return;
            }

            if (RangeOf(pointDirection.x, playerDirection.x, 0.001f)) {
                if (index + 1 > fleePoints.targets.Length - 1) {
                    index = 0;
                } else {
                    index++;
                }
                fleePoints.targetIndex = index;
                //Debug.Log("Player is here, move to = " + fleePoints.targets[index].gameObject.name);
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
            RangeOf(movManager.transform.position.y, room.targets[room.targetIndex].position.y, 0.75f)) {
            if ((room.targets.Length - 1) == room.targetIndex) {
                room.targetIndex = 0;
            } else {
                room.targetIndex += 1;
            }
        }
    }

    private void DetectionEventState(Enemy enemy) {
        if (enemy.detectionManager.detectionState == DetectionState.None) {
            enemy.movementManager.BasicMovement();
        } else if (enemy.detectionManager.detectionState == DetectionState.Alert) {
            enemy.movementManager.AlertMovement();
        } else if (enemy.detectionManager.detectionState == DetectionState.Spoted) {
            enemy.movementManager.SpotMovement();
        } else if (enemy.detectionManager.detectionState == DetectionState.Flee) {
            enemy.movementManager.FleeMovement();
        } else if (enemy.detectionManager.detectionState == DetectionState.Freeze) {
            enemy.movementManager.FreezeMovement();
        } else {
            Debug.Log("error : enemy has no detection state");
            enemy.sprite.color = Color.blue;
        }
    }

    private void AnimationStateMachine(Enemy enemy)
    {
        Vector3 targetDistance = FindTargetDirection(
            enemy.movementManager.spritePos.position, 
            enemy.movementManager.target.position);

        bool isAtTargetPosition = false;
        bool isClimbing = enemy.movementManager.isEndClimbing || enemy.movementManager.isClimbing;
        
        if (targetDistance.x >= 0) {
            if (targetDistance.x < 0.1f && RangeOf(targetDistance.y, 0f, 0.80f)) {
                isAtTargetPosition = true;
            }
        } else if (targetDistance.x <= 0) {
            if (targetDistance.x > -0.1f && RangeOf(targetDistance.y, 0f, 0.80f)) {
                isAtTargetPosition = true;
            }
        }

        animatorController.Idle(enemy, isAtTargetPosition, isClimbing);
        animatorController.Fight(enemy, isClimbing);
        animatorController.Scared(enemy, isAtTargetPosition, isClimbing);
        animatorController.Moving(enemy, isAtTargetPosition, isClimbing);
        animatorController.Climbing(enemy, targetDistance, isAtTargetPosition, isClimbing);
    }
}