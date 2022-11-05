using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class GlowOnTouch : MonoBehaviour
{
    private SpriteRenderer _glowShader;
    [ColorUsageAttribute(true, true)][SerializeField] private Color _color;
    [SerializeField] private float _outlinethickness = 1.0f;

    public void SetOutlineColor(Color color, bool setDefault = true)
    {
        if (setDefault)
            _glowShader.material.SetColor("_OutlineColor", _color);
        else
            _glowShader.material.SetColor("_OutlineColor", color);
    }

    private void Awake()
    {
        _glowShader = GetComponent<SpriteRenderer>();
        _glowShader.material = Resources.Load("GlowOutline", typeof(Material)) as Material;
        _glowShader.material.SetColor("_OutlineColor", _color);
    }

    public void Trigger(bool isTrigger)
    {
        _glowShader.material.SetFloat("_Outlinethickness", isTrigger ? _outlinethickness : 0.0f);
    }
}
