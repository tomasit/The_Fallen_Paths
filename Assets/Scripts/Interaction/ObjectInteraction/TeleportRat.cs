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
        
        while (NTime < 1.0f) {
            _animStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            NTime = _animStateInfo.normalizedTime;
            yield return null;
        }
        _player.gameObject.SetActive(false);

        yield return new WaitForSeconds(_timeToWait);
        _animator.Play("Idle");
        _player.transform.position = _positionToTeleport.position;
        _player.gameObject.SetActive(true);
        NTime = 0f;
    }

    public override void Save()
    {
    }

    public override void Load()
    {
    }
}
