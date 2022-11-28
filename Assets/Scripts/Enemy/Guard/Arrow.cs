using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static EnemyInfo;

public class Arrow : MonoBehaviour
{
    public float speed = 1f;
    public uint damages = 1;
    public Rigidbody2D rigidbody;
    //public GameObject particles;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.AddForce(transform.right.normalized * speed, ForceMode2D.Impulse);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider != null) {
            var layerName = collider.gameObject.layer;
            if (layerName == LayerMask.NameToLayer("Ground") || layerName == LayerMask.NameToLayer("Stairs") || 
                layerName == LayerMask.NameToLayer("Lader") || layerName == LayerMask.NameToLayer("LastLader") || 
                layerName == LayerMask.NameToLayer("PlayerLader")) {
                return;
            }
            if (collider.gameObject.GetComponent<BasicHealthWrapper>() != null) {
                collider.gameObject.GetComponent<BasicHealthWrapper>().Hit(damages);
            }
            
            //instanciate particules
            //GameObject newParticles = Object.Instantiate(particles, transform.position, transform.rotation);
            Object.Destroy(gameObject);
        }
    }
}
