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
            enemy.interactionManager.damage = Damage[enemy.type];
            enemy.interactionManager.coolDown = CoolDown[enemy.type];
        }
    }

    void Update()
    {
        foreach(var enemy in Enemies) {
            //si il est desactivé
            //if (enemy.activeSelf == false)
            //    continue;
            if (enemy.dectionManager.detectionState == DetectionState.None)
                RoomMovement(enemy);
            DetectionEventState(enemy);
        }
    }

    private void RoomMovement(Enemy enemy)
    {
        if (enemy.movementManager.transform.position.x < enemy.roomProprieties.rooomCoordonates.x) {
            enemy.movementManager.direction = 1;
            enemy.dectionManager.direction = Vector2.right;
        }
        if (enemy.movementManager.transform.position.x > enemy.roomProprieties.rooomCoordonates.y) {
            enemy.movementManager.direction = -1;
            enemy.dectionManager.direction = Vector2.left;
        }
    }

    private void DetectionEventState(Enemy enemy) {
        if (enemy.dectionManager.detectionState == DetectionState.None) {
            enemy.sprite.color = Color.green;
            enemy.movementManager.BasicMovement();
        } else if (enemy.dectionManager.detectionState == DetectionState.Alert) {
            enemy.sprite.color = Color.yellow;
            enemy.movementManager.AlertMovement();
        } else if (enemy.dectionManager.detectionState == DetectionState.Spoted) {
            enemy.sprite.color = Color.red;
            enemy.movementManager.SpotMovement();
        } else {
            Debug.Log("error : enemy has no detection state");
            enemy.sprite.color = Color.blue;
        }
    }
}