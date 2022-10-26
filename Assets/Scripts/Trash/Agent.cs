using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Agent : MonoBehaviour
{
    [SerializeField] Transform _target;
    [SerializeField] Vector3 _targetPosition;
    [HideInInspector] private NavMeshAgent agent;
    [HideInInspector] private GameObject AgentSprite;

    void Start()
    {
        _targetPosition = gameObject.transform.position;

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Update()
    {
        agent.SetDestination(_targetPosition);
    }

    public void SetSpeed(float speed)
    {
        agent.speed = speed;
    }

    public void SetTarget(Transform target, Transform destination)
    {
        float height = destination.gameObject.GetComponent<SpriteRenderer>().bounds.size.y;

        var adjust = 0f;
        if (target.gameObject.name == "Player") {
            adjust = destination.localPosition.y;
        }

        Vector3 adjustedTarget = new Vector3(
            target.position.x, 
            target.position.y - (height / 2f) - adjust,
            target.position.z);

        _target = target;
        _targetPosition = adjustedTarget;
    }

}
