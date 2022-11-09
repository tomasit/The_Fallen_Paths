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

        bool isAtTargetPosition = false;
        bool isClimbing = enemy.movementManager.isEndClimbing || enemy.movementManager.isClimbing;
        
        Debug.Log("targetDistance.x = " + targetDistance.x);
        Debug.Log("targetDistance.y = " + targetDistance.y);
        if (targetDistance.x > 0) {
            if (targetDistance.x < 0.1f && RangeOf(targetDistance.y, 0f, 0.80f)) {
                isAtTargetPosition = true;
            }
        } else if (targetDistance.x < 0) {
            if (targetDistance.x > -0.1f && RangeOf(targetDistance.y, 0f, 0.80f)) {
                isAtTargetPosition = true;
            }
        }

        if (isAtTargetPosition && 
            (enemy.detectionManager.detectionState == DetectionState.None ||
            enemy.detectionManager.detectionState == DetectionState.Freeze) &&
            !isClimbing
            ) {
            enemy.animator.SetTrigger("Idle");
        }
        if (enemy.movementManager.isAtDistanceToInteract && 
            (enemy.detectionManager.detectionState == DetectionState.Alert || 
            enemy.detectionManager.detectionState == DetectionState.Spoted) &&
            !isClimbing
            ) {
            enemy.animator.SetTrigger("Ready");
        }
        if (isAtTargetPosition && 
            enemy.detectionManager.detectionState == DetectionState.Flee &&
            !isClimbing) {
            enemy.animator.SetTrigger("Scared");
        }
        if (!isAtTargetPosition &&
            (enemy.detectionManager.detectionState == DetectionState.Alert ||
            enemy.detectionManager.detectionState == DetectionState.None) &&
            !isClimbing /*il a pas pris de hit*/) {
                enemy.animator.SetTrigger("Walking");
        }
        if (!enemy.movementManager.isAtDistanceToInteract &&
            (enemy.detectionManager.detectionState == DetectionState.Spoted ||
            enemy.detectionManager.detectionState == DetectionState.Flee) &&
            !isClimbing /*il a pas pris de hit*/) {
                enemy.animator.SetTrigger("Running");
                //sinon il doit être en mode ready to nicker des mères
        }
        
        if (enemy.movementManager.collisionObj != null) {
            if (enemy.movementManager.isEndClimbing && (targetDistance.y < -1f)) {
                enemy.animator.SetTrigger("SitDown");
            }
            if (enemy.movementManager.collisionObj.gameObject.layer == LayerMask.NameToLayer("Lader") && 
            enemy.movementManager.isClimbing && !RangeOf(targetDistance.y, 0f, 1f) && 
            !enemy.movementManager.isEndClimbing) {
                enemy.animator.SetBool("Climbing", true);
            }
            if (enemy.movementManager.isEndClimbing && enemy.movementManager.isClimbing && 
            !(targetDistance.y < -1f)) {
                enemy.animator.SetTrigger("StandUp");
                enemy.animator.SetBool("Climbing", false);
                enemy.movementManager.isClimbing = false;
            }
        } else {
            enemy.animator.SetBool("Climbing", false);
            if (!enemy.movementManager.isEndClimbing) {
                enemy.movementManager.isClimbing = false;
            }
        }

        var currentAnimationName = enemy.animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;

        //Debug.Log("isAtTargetPosition = " + isAtTargetPosition);
        
        //si c le player faut soustraire l'offset en X de l'attaque
        if (isAtTargetPosition && (currentAnimationName == "sword_climbing")) {
            Debug.Log("stop anim");
            enemy.animator.speed = 0;
        } else {
            enemy.animator.speed = 1;
        }

        //faire des conditions pour les lader ici
        
        //health manager sur les enemies
        /*if (//il s'est fait hit, savedHealth <= Health
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
        }*/       
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