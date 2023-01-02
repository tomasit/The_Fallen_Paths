using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using static EnemyInfo;

public class Agent : MonoBehaviour
{
    [SerializeField] Transform _target;
    [SerializeField] public Vector3 targetPosition;
    [HideInInspector] private NavMeshAgent _agent;
    [HideInInspector] private Transform _agentSprite;

    void Start()
    {
        targetPosition = gameObject.transform.position;

        _agent = GetComponent<NavMeshAgent>();
        _agentSprite = transform.GetChild(0).GetComponent<SpriteRenderer>().transform;
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
    }

    void Update()
    {
        _agent.SetDestination(targetPosition);
    }

    public void DisableAgent()
    {
        _target = transform;
        targetPosition = _target.position;

    }

    public void SetSpeed(float speed)
    {
        _agent.speed = speed;
    }

    //offest : use it for sprite Y offset (between agent & real sprite pos)
    public void SetTarget(Transform target, Vector3 offset)
    {
        if (target == null) {
            Debug.Log("Agent target is null");
            return;
        }
        
        Vector3 adjustedTarget = Vector3.zero;

        if (target.gameObject.name == "Player" || target.gameObject.name == "TargetPos") {
            Vector3 targetDirection = FindTargetDirection(_agentSprite.position, target.position);
            
            Vector3 distanceToPlayer = new Vector3(
                target.position.x + (DistanceToInteract.x * (targetDirection.x > 0 ? -1 : 1)), 
                target.position.y + (DistanceToInteract.y * (targetDirection.y > 0 ? 1 : -1)), 
                target.position.z);
            adjustedTarget = distanceToPlayer - new Vector3(0f, offset.y, 0f);
        } else {
            adjustedTarget = target.position;
        }
        
        _target = target;
        targetPosition = adjustedTarget;
    }

}
