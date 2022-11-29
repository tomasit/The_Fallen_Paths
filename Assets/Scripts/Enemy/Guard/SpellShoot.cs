
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static EnemyInfo;

public class SpellShoot : MonoBehaviour
{
    public float speed = 1f;
    public uint damages = 1;
    private Animator animator;
    private Rigidbody2D rb;
    private Collider2D ownCollider;
    [SerializeField] private GameObject particles;

    void Start()
    {
        animator = GetComponent<Animator>();
        ownCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(transform.right.normalized * speed, ForceMode2D.Impulse);

        // Ignore the collider of his father
        if (transform.parent.gameObject.GetComponent<Collider2D>() != null) {
            Physics2D.IgnoreCollision(ownCollider, transform.parent.gameObject.GetComponent<Collider2D>(), true);
        }
    }

    void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (otherCollider != null) {
            var layerName = otherCollider.gameObject.layer;
            if (layerName == LayerMask.NameToLayer("Ground") || layerName == LayerMask.NameToLayer("Stairs") || 
                layerName == LayerMask.NameToLayer("Lader") || layerName == LayerMask.NameToLayer("LastLader") || 
                layerName == LayerMask.NameToLayer("PlayerLader")) {
                return;
            }
            if (otherCollider.gameObject.GetComponent<BasicHealthWrapper>() != null) {
                otherCollider.gameObject.GetComponent<BasicHealthWrapper>().Hit(damages);
            }
            
            GameObject newParticles = Object.Instantiate(particles, transform.position, transform.rotation);
            Object.Destroy(gameObject);
        }
    }
}
