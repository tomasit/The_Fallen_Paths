using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using static EnemyInfo;

public class AnimatorStateMachine : MonoBehaviour
{
    public void Idle(Enemy enemy, bool isAtTargetPosition, bool isClimbing)
    {
        if (isAtTargetPosition && 
            (enemy.detectionManager.GetState() == DetectionState.None ||
            enemy.detectionManager.GetState() == DetectionState.Freeze) &&
            !isClimbing
            ) {
            enemy.animator.SetTrigger("Idle");
        }

        if (!enemy.movementManager.HasMovedFromLastFrame() && 
        (enemy.detectionManager.GetState() == DetectionState.None || 
        enemy.detectionManager.GetState() == DetectionState.Freeze)) {
            enemy.animator.SetTrigger("Idle");
        }
    }

    public void Fight(Enemy enemy, bool isClimbing)
    {
        if (!enemy.movementManager.HasMovedFromLastFrame() && 
            (enemy.detectionManager.GetState() == DetectionState.Alert || 
            enemy.detectionManager.GetState() == DetectionState.Spoted) &&
            !isClimbing
            ) {
            enemy.animator.SetTrigger("Ready");
        }

        if (!enemy.movementManager.HasMovedFromLastFrame() && 
        (enemy.detectionManager.GetState() == DetectionState.Alert || 
        enemy.detectionManager.GetState() == DetectionState.Spoted)) {
            enemy.animator.SetTrigger("Ready");
        }
    }

    public void Moving(Enemy enemy, bool isAtTargetPosition, bool isClimbing)
    {
        if (!enemy.movementManager.HasMovedFromLastFrame()) {
            return;
        }
        if (!isAtTargetPosition &&
            (enemy.detectionManager.GetState() == DetectionState.Alert ||
            enemy.detectionManager.GetState() == DetectionState.None) &&
            !isClimbing /*il a pas pris de hit*/) {
                enemy.animator.SetTrigger("Walking");
        }
        if (!enemy.movementManager.isAtDistanceToInteract &&
            (enemy.detectionManager.GetState() == DetectionState.Spoted ||
            enemy.detectionManager.GetState() == DetectionState.Flee) &&
            !isClimbing /*il a pas pris de hit*/) {
                enemy.animator.SetTrigger("Running");
        }
    }

    public void Scared(Enemy enemy, bool isAtTargetPosition, bool isClimbing)
    {
        if (isAtTargetPosition && 
            enemy.detectionManager.GetState() == DetectionState.Flee &&
            !isClimbing) {
            enemy.animator.SetTrigger("Scared");
        }
    }

    public void Climbing(Enemy enemy, Vector3 targetDistance, bool isAtTargetPosition, bool isClimbing)
    {
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

            if (!enemy.movementManager.HasMovedFromLastFrame()) {
                enemy.animator.speed = 0;
            } else {
                enemy.animator.speed = 1;
            }
        } else {
            enemy.animator.SetBool("Climbing", false);
            if (!enemy.movementManager.isEndClimbing) {
                enemy.movementManager.isClimbing = false;
            }
        }
    }

}