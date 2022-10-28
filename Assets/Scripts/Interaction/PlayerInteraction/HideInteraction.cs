using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideInteraction : MonoBehaviour
{
    private PlayerController _controller;
    private Animator _animator;

    private void Start()
    {
        _controller = GetComponent<PlayerController>();
        _animator = GetComponent<Animator>();
    }

    public bool IsHide()
    {
        return true;
    }

    public void Hide()
    {
        if (_animator.GetBool("Dead"))
            return;
        if (!_controller.isGrounded())
            return;
        if (_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "PlayerHit")
            return;

        // _controller.BlockInput(true);
        // _animator.SetBool("HideVisible", !_animator.GetBool("HideVisible"));
    }

    public void LitPlaceAlight(bool isAlight)
    {
    }
}
