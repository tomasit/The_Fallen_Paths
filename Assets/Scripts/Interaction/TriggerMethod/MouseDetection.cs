using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseDetection : TriggerProcessor
{
    [SerializeField] private bool _stopOnTrigger = false;
    [SerializeField] private bool _enabled = false;
    private GlowOnTouch _glowEffect;

    private void Start()
    {
        _glowEffect = GetComponent<GlowOnTouch>();
    }

    public override void Trigger()
    {
        InteractionProcessor processor = GetComponent<InteractionProcessor>();
        if (processor != null)
        {
            processor.Interact();
        }
    }

    private void Glow(bool glow)
    {
        if (_glowEffect != null)
            _glowEffect.Trigger(glow);
    }

    private void Update()
    {
        if (!_enabled)
            return;

        RaycastHit2D hit;
        if ((hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, float.PositiveInfinity)).collider != null)
        {  
            if (hit.transform == transform)
            {
                if (_glowEffect != null)
                    _glowEffect.Trigger(true);
                if (Input.GetMouseButtonDown(0))
                {
                    Trigger();
                    Glow(false);
                    if (_stopOnTrigger)
                        _enabled = false;
                }
            }
            else
            {
                Glow(false);
            }
        }
        else
        {
            Glow(false);
        }
    }
}