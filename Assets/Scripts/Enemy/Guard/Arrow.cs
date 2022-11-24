using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static EnemyInfo;

public class Arrow : MonoBehaviour
{
    public float speed = 1f;
    public int damages = 1;
    public Rigidbody2D rigidbody;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
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
                gameObject.GetComponent<BasicHealthWrapper>().Hit((uint)damages);
            }
            //instanciate particules
            Object.Destroy(gameObject);
        }
    }
}
