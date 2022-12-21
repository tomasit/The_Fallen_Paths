using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TeleportRat : AInteractable
{
    private PlayerController _player;
    private Animator _animator;
    private AnimatorStateInfo _animStateInfo;
    private float NTime = 0f;
    [SerializeField] private Transform _positionToTeleport;
    [SerializeField] private float _timeToWait = 1.5f;

    void Start()
    {
        _player = (PlayerController)FindObjectOfType(typeof(PlayerController));
        _animator = _player.gameObject.GetComponent<Animator>();
    }

    public override void Interact()
    {
        StartCoroutine(TeleportRatCoroutine());
    }

    private IEnumerator TeleportRatCoroutine()
    {
        //play sound rat dans canalisation
        _animator.SetTrigger("JumpInTube");
        _player.BlockInput(true);
        _player.gameObject.GetComponent<ComputeInteraction>().BlockInput(true);
        
        while (NTime < 1.0f) {
            _animStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            NTime = _animStateInfo.normalizedTime;
            yield return null;
        }
        // block input

        // make player disappear withour set active false
        _player.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        _player.gameObject.GetComponent<Collider2D>().isTrigger = true;
        var color = _player.gameObject.GetComponent<SpriteRenderer>().material.color;
        color.a = 0.0f;
        _player.gameObject.GetComponent<SpriteRenderer>().material.color = color;

        yield return new WaitForSeconds(_timeToWait);
        _player.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        _player.gameObject.GetComponent<Collider2D>().isTrigger = false;
        color = _player.gameObject.GetComponent<SpriteRenderer>().material.color;
        color.a = 1.0f;
        _player.gameObject.GetComponent<SpriteRenderer>().material.color = color;

        _player.BlockInput(false);
        _player.gameObject.GetComponent<ComputeInteraction>().BlockInput(false);

        _player.transform.position = _positionToTeleport.position;
        NTime = 0f;
    }

    public override void Save()
    {
    }

    public override void Load()
    {
    }
}
