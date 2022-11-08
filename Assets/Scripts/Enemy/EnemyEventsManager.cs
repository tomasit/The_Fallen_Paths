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

    void Start()
    {
        player = ((PlayerController)FindObjectOfType(typeof(PlayerController))).transform;
        


        foreach(var enemy in Enemies) {
            //enemy.uuid = Guid.NewGuid().ToString();

            IgnoreLayers(enemy);

            //enemy.interactionManager.damage = Damage[enemy.type];
            //enemy.interactionManager.coolDown = CoolDown[enemy.type];
            
            enemy.movementManager.target = enemy.roomProprieties.targets[0];
        }
    }

    void Update()
    {
        foreach(var enemy in Enemies) {
            if (!enemy.enabled) {
                continue;   
            }
            if (enemy.detectionManager.detectionState == DetectionState.None) {
                RoomTargetPoints(enemy);
                enemy.movementManager.target = enemy.roomProprieties.targets[enemy.roomProprieties.targetIndex];
            }
            if (enemy.detectionManager.detectionState == DetectionState.Flee) {
                FleeTargetPoints(enemy);
                enemy.movementManager.target = enemy.fleePoints.targets[enemy.fleePoints.targetIndex];
            }
            RaycastDirection(enemy);
            DetectionEventState(enemy);
            AnimationController(enemy);
        }
    }

    private void AnimationController(Enemy enemy)
    {
        Vector3 targetDistance = FindTargetDirection(
            enemy.movementManager.spritePos.position, 
            enemy.movementManager.target.position);

        bool isAtDistanceToInteract = false;
        
        if (targetDistance.x > 0) {
            if (targetDistance.x < 0.1f && RangeOf(targetDistance.y, 0f, 0.80f)) {
                isAtDistanceToInteract = true;
            }
        } else if (targetDistance.x < 0) {
            if (targetDistance.x > -0.1f && RangeOf(targetDistance.y, 0f, 0.80f)) {
                isAtDistanceToInteract = true;
            }
        }

        if (isAtDistanceToInteract && 
            (enemy.detectionManager.detectionState == DetectionState.None ||
            enemy.detectionManager.detectionState == DetectionState.Freeze) &&
            enemy.movementManager.collisionObj == null
            ) {
            enemy.animator.SetTrigger("Idle");
        }
        if (isAtDistanceToInteract && 
            (enemy.detectionManager.detectionState == DetectionState.Alert || 
            enemy.detectionManager.detectionState == DetectionState.Spoted) &&
            enemy.movementManager.collisionObj == null
            ) {
            enemy.animator.SetTrigger("Ready");
        }
        if (isAtDistanceToInteract && 
            enemy.detectionManager.detectionState == DetectionState.Flee &&
            enemy.movementManager.collisionObj == null) {
            enemy.animator.SetTrigger("Scared");
        }
        if (!isAtDistanceToInteract  &&
            (enemy.detectionManager.detectionState == DetectionState.Alert ||
            enemy.detectionManager.detectionState == DetectionState.None) &&
            enemy.movementManager.collisionObj == null
            /*il a pas pris de hit*/) {
                enemy.animator.SetTrigger("Walking");
        }
        if (!isAtDistanceToInteract &&
            (enemy.detectionManager.detectionState == DetectionState.Spoted ||
            enemy.detectionManager.detectionState == DetectionState.Flee) &&
            enemy.movementManager.collisionObj == null
            /*il a pas pris de hit*/) {
                enemy.animator.SetTrigger("Running");
        }

        //faire des conditions pour les lader ici

        /*
        if (//il s'est fait hit, savedHealth <= Health
        ) {
            enemy.animator.SetTrigger("Hit");
        }
        if (//healthMnager <= 0
        ) {
            enemy.animator.SetTrigger("Dead");
        }
        if (//son collider trigger avec un collider lader
        ) {
            //enemy.animator.SetTrigger("Climbing");
        } */       
    }

    private void IgnoreLayers(Enemy enemy)
    {
        foreach (LayerMask layer in layersToIgnore)
        {
            // NOTE : if the layerMask is the 31, it didn't work
            int layerValue = (int)Mathf.Log(layer.value, 2);
            Physics2D.IgnoreLayerCollision(layerValue, enemy.movementManager.gameObject.layer, true);
        }
    }

    private void RaycastDirection(Enemy enemy)
    {
        if (enemy.movementManager.target == null) {
            //Debug.Log("MovementManager target is null (eventManager issue)");
            return;
        }

        Vector3 directionPoint = FindTargetDirection(enemy.movementManager.gameObject.transform.position, enemy.movementManager.target.position);

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
}