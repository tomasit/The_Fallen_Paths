using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] public int health;
    [SerializeField] public int maxHealth = 2;

    private void Start()
    {
        if (health == 0)
            health = maxHealth;
    }

    public void Hit(uint damage, System.Action actionWhenHit = null, System.Action actionIfDead = null)
    {
        health -= (int)damage;

        if (actionWhenHit != null)
            actionWhenHit();
        if (actionIfDead != null && isDead())
            actionIfDead();
    }

    public void Heal(uint healValue)
    {
        health = Mathf.Min(health + (int)healValue, maxHealth);
    }

    public bool isDead()
    {
        return health <= 0;
    }
}
