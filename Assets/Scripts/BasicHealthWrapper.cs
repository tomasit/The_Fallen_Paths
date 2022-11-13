using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class BasicHealthWrapper : MonoBehaviour
{
    private Health _health;
    [SerializeField] private Animator _animator;
    [SerializeField] private bool externalAnimator = false;

    void Start()
    {
        _health = GetComponent<Health>();
        if (!externalAnimator) {
            _animator = GetComponent<Animator>();
        }
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
        _animator.SetBool("Dead", false);
//        _animator.SetTrigger("Heal");
    }

    public bool isDead() 
    {
        return _health.isDead();
    }

    public void SetAnimator(Animator animator)
    {
        _animator = animator;
    }

    public void SetMaxHealth(int health)
    {
        if (_health == null) {
            _health = GetComponent<Health>();
        }
        _health.health = health;
        _health.maxHealth = health;
    }
}
