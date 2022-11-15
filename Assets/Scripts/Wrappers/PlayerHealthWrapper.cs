using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealthWrapper : BasicHealthWrapper
{
    [SerializeField] private UnityEvent _deathEvent;

    public override void Hit(uint damage)
    {
        _health.Hit(damage,
            () => {_animator.SetTrigger("Hit");},
            () => {_animator.SetBool("Dead", true); _deathEvent.Invoke();}
        );
    }
}
