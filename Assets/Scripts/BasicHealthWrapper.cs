using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health), typeof(Animator))]
public class BasicHealthWrapper : MonoBehaviour
{
    private Health _health;
    private Animator _animator;

    void Start()
    {
        _health = GetComponent<Health>();
        _animator = GetComponent<Animator>();
    }

    public void Hit(uint damage)
    {
        _health.Hit(damage, 
            () => {_animator.SetTrigger("Hit");},
            () => {_animator.SetBool("Dead", true);}
        );
    }

    public void Heal(uint healValue)
    {
        _health.Heal(healValue);
        _animator.SetTrigger("Heal");
    }

    public bool isDead() 
    {
        return _health.isDead();
    }
}
