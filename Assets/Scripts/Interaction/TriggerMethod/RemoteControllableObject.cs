using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GlowOnTouch))]
[RequireComponent(typeof(Collider2D))]
public class RemoteControllableObject : TriggerProcessor
{
    [SerializeField] private bool _isTrigger = true;
    [SerializeField] private GameObject _particle = null;
    [ColorUsageAttribute(true, true)][SerializeField] private Color _glowColor;
    private InteractionProcessor _processor;

    private void Awake()
    {
        _processor = GetComponent<InteractionProcessor>();
        GetComponent<Collider2D>().isTrigger = _isTrigger;
        _particle.SetActive(false);
    }

    public override void Trigger()
    {
        if (_processor != null)
        {
            _processor.Interact();
        }
    }

    public bool IsTriggerable()
    {
        return !_processor._interact;
    }

    public void ActiveOutline(bool active)
    {
        if (_processor._interact)
            return;
        GetComponent<GlowOnTouch>().SetOutlineColor(_glowColor, !active);
        GetComponent<GlowOnTouch>().Trigger(active);
    }

    public void UnactiveParticle()
    {
        if (_particle != null)
            _particle.SetActive(false);
    } 

    public void RateUpParticle()
    {
        if (_particle != null && !_processor._interact)
        {
            if (!_particle.activeSelf)
                _particle.SetActive(true);
            var emission = _particle.GetComponent<ParticleSystem>().emission;
            emission.rateOverTime = 1;
        }
    }

    public void RateDownParticle()
    {
        if (_particle != null && !_processor._interact)
        {
            if (!_particle.activeSelf)
                _particle.SetActive(true);
            var emission = _particle.GetComponent<ParticleSystem>().emission;
            emission.rateOverTime = 0;
        }
    }
}