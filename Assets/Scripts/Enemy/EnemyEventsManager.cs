using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using static EnemyInfo;

public class EnemyEventsManager : MonoBehaviour
{
    public Enemy[] Enemies;

    void Start()
    {
        foreach(var enemy in Enemies) {
            //enemy.uuid = Guid.NewGuid().ToString();

            enemy.interactionManager.damage = Damage[enemy.type];
            enemy.interactionManager.coolDown = CoolDown[enemy.type];
            
            enemy.movementManager.target = enemy.roomProprieties.targets[0];
        }
    }

    void Update()
    {
        foreach(var enemy in Enemies) {
            if (enemy.detectionManager.detectionState == DetectionState.None) {
                RoomTargetPoints(enemy);
                enemy.movementManager.target = enemy.roomProprieties.targets[enemy.roomProprieties.targetIndex];
            }
            RaycastDirection(enemy);
            DetectionEventState(enemy);
        }
    }

    private bool RangeOf(float posAim, float posCompare, float range)
    {
        return (posCompare - range < posAim) && (posAim < posCompare + range);
    }

    private void RaycastDirection(Enemy enemy)
    {
        Vector3 directionPoint = enemy.movementManager.FindTargetDirection(enemy.movementManager.target.position);

        if (directionPoint.x > 0) {
            enemy.detectionManager.SetRayCastDirection(Vector2.right);
        } else {
            enemy.detectionManager.SetRayCastDirection(Vector2.left);
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
            enemy.sprite.color = Color.green;
            enemy.movementManager.BasicMovement();
        } else if (enemy.detectionManager.detectionState == DetectionState.Alert) {
            enemy.sprite.color = Color.yellow;
            enemy.movementManager.AlertMovement();
        } else if (enemy.detectionManager.detectionState == DetectionState.Spoted) {
            enemy.sprite.color = Color.red;
            enemy.movementManager.SpotMovement();
        } else {
            Debug.Log("error : enemy has no detection state");
            enemy.sprite.color = Color.blue;
        }
    }
}