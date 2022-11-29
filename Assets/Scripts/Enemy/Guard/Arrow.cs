using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static EnemyInfo;

public class Arrow : MonoBehaviour
{
    public float speed = 1f;
    public uint damages = 1;
    private Rigidbody2D rb;
    private Collider2D ownCollider;
    //[SerializeField] private GameObject particles;

    void Start()
    {
        ownCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(transform.right.normalized * speed, ForceMode2D.Impulse);

        // Ignore the collider of his father
        if (transform.parent.gameObject.GetComponent<Collider2D>() != null) {
            Physics2D.IgnoreCollision(ownCollider, transform.parent.gameObject.GetComponent<Collider2D>(), true);
        }
    }

    void OnTriggerEnter2D(Collider2D colliderOther)
    {
        if (colliderOther != null) {
            var layerName = colliderOther.gameObject.layer;
            if (layerName == LayerMask.NameToLayer("Ground") || layerName == LayerMask.NameToLayer("Stairs") || 
                layerName == LayerMask.NameToLayer("Lader") || layerName == LayerMask.NameToLayer("LastLader") || 
                layerName == LayerMask.NameToLayer("PlayerLader")) {
                return;
            }
            if (colliderOther.gameObject.GetComponent<BasicHealthWrapper>() != null) {
                colliderOther.gameObject.GetComponent<BasicHealthWrapper>().Hit(damages);
            }
            
            //GameObject newParticles = Object.Instantiate(particles, transform.position, transform.rotation);
            Object.Destroy(gameObject);
        }
    }
}
