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
            (enemy.detectionManager.detectionState == DetectionState.None ||
            enemy.detectionManager.detectionState == DetectionState.Freeze) &&
            !isClimbing
            ) {
            enemy.animator.SetTrigger("Idle");
        }
    }
    public void Fight(Enemy enemy, bool isClimbing)
    {
        if (enemy.movementManager.isAtDistanceToInteract && 
            (enemy.detectionManager.detectionState == DetectionState.Alert || 
            enemy.detectionManager.detectionState == DetectionState.Spoted) &&
            !isClimbing
            ) {
            enemy.animator.SetTrigger("Ready");
        }
    }
    public void Scared(Enemy enemy, bool isAtTargetPosition, bool isClimbing)
    {
        if (isAtTargetPosition && 
            enemy.detectionManager.detectionState == DetectionState.Flee &&
            !isClimbing) {
            enemy.animator.SetTrigger("Scared");
        }
    }
    public void Moving(Enemy enemy, bool isAtTargetPosition, bool isClimbing)
    {
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
        } else {
            enemy.animator.SetBool("Climbing", false);
            if (!enemy.movementManager.isEndClimbing) {
                enemy.movementManager.isClimbing = false;
            }
        }

        //Debug.Log("targetDistance.x = " + targetDistance.x);
        //Debug.Log("targetDistance.y = " + targetDistance.y);

        /*var currentAnimationName = enemy.animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;

        //Debug.Log("isAtTargetPosition = " + isAtTargetPosition);
        
        //si c le player faut soustraire l'offset en X de l'attaque
        if (isAtTargetPosition && (currentAnimationName == "sword_climbing")) {
            Debug.Log("stop anim");
            enemy.animator.speed = 0;
        } else {
            enemy.animator.speed = 1;
        }*/
    }
    public void Damages(Enemy enemy)
    {
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

}