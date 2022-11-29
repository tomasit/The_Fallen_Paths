using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static EnemyInfo;

public class SpellImpact : MonoBehaviour
{
    private Animator animator;
    private AnimatorStateInfo animStateInfo;
    private float NTime;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        animStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        NTime = animStateInfo.normalizedTime;
        if(NTime > 1.0f) {
            Object.Destroy(gameObject);
        }
    }
}
