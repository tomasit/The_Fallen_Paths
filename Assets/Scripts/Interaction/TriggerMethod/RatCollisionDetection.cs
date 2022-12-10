using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GlowOnTouch))]
[RequireComponent(typeof(Collider2D))]
public class RatCollisionDetection : TriggerProcessor
{
    [SerializeField] private bool _isTrigger = true;
    [SerializeField] private Vector3 _textOffset;
    [HideInInspector] public string _displayedText;
    [SerializeField] private LayerMask _triggerLayer;
    private InteractionDisplayText _UIDisplayedText = null;
    private PlayerController player;

    private void Awake()
    {
        _UIDisplayedText = (InteractionDisplayText)FindObjectOfType(typeof(InteractionDisplayText), true);
        if (_UIDisplayedText == null)
            Debug.Log("No InteractionDisplayText find in canvas. Think about add this to your project");

        GetComponent<Collider2D>().isTrigger = _isTrigger;
    }

    void Start()
    {
        player = (PlayerController)FindObjectOfType(typeof(PlayerController));
    }

    // this function is used to compute which interactable component has the
    // heaviest weight then return the description.
    // based on the fact that an interaction who has no weight has no description too
    // <!> if two interactions has the same weight this function return the first one find
    // so when attribute weight, declare one enum per interaction
    private string GetDescription(GameObject obj)
    {
        AInteractable[] interactions = obj.transform.GetComponents<AInteractable>();
        AInteractable heavierInteraction = null;
        AInteractable.DescriptionHeight baseHeight = AInteractable.DescriptionHeight.NONE;

        foreach (var i in interactions)
        {
            if (i.GetDescriptionHeight() > baseHeight)
            {
                baseHeight = i.GetDescriptionHeight();
                heavierInteraction = i;
            }
        }

        return heavierInteraction != null ? heavierInteraction.GetDescription() : "";
    }

    public override void Trigger()
    {
        InteractionProcessor processor = GetComponent<InteractionProcessor>();
        if (processor != null && player.GetPlayerType() == PlayerType.RAT)
        {
            processor.Interact();
//            SetDescription(GetDescription(transform.gameObject));
        }
    }

    public void SetDescription(string description)
    {
        if (_UIDisplayedText != null)
        {
//            _UIDisplayedText.SetText(description);
        }
    }

    private void OnTriggerEnter2D(Collider2D hit)
    {
        if (player.GetPlayerType() != PlayerType.RAT) {
            return;
        }
        if (!_isDisable)
        {
            if ((_triggerLayer & 1 << hit.gameObject.layer) == 1 << hit.gameObject.layer)
            {
                GetComponent<GlowOnTouch>().Trigger(true);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D hit)
    {
        if (player.GetPlayerType() != PlayerType.RAT) {
            return;
        }
        if (!_isDisable)
        {
            if ((_triggerLayer & 1 << hit.gameObject.layer) == 1 << hit.gameObject.layer)
            {
                GetComponent<GlowOnTouch>().Trigger(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D hit)
    {
        if (!_isDisable)
        {
            if ((_triggerLayer & 1 << hit.gameObject.layer) == 1 << hit.gameObject.layer)
            {
                GetComponent<GlowOnTouch>().Trigger(false);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D hit)
    {
        if (!_isDisable)
        {
            if ((_triggerLayer & 1 << hit.gameObject.layer) == 1 << hit.gameObject.layer)
            {
                if (player.GetPlayerType() == PlayerType.RAT) {
                    GetComponent<GlowOnTouch>().Trigger(true);
                } else {
                    GetComponent<GlowOnTouch>().Trigger(false);
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D hit)
    {
        if (!_isDisable)
        {
            if ((_triggerLayer & 1 << hit.gameObject.layer) == 1 << hit.gameObject.layer)
            {
                GetComponent<GlowOnTouch>().Trigger(false);
            }
        }
    }
}