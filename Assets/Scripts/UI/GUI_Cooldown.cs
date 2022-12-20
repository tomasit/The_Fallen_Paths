using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GUI_Cooldown : MonoBehaviour
{
    public int PowerIndex;

    [SerializeField] private Image imageCD;
    [SerializeField] private TMP_Text textCD;
    [SerializeField] private PowerManager _powerManager;
    private float cd = 0;
    private float maxDuration = 0;

    private void Start()
    {
        _powerManager = FindObjectOfType<PowerManager>();
        textCD.gameObject.SetActive(false);
        maxDuration = _powerManager._powers[PowerIndex].cooldownDuration;
    }

    private void Update()
    {
        /*if (_powerManager == null) {
            _powerManager = FindObjectOfType<PowerManager>();
            maxDuration = _powerManager._powers[PowerIndex].cooldownDuration;
        }*/

        cd = _powerManager._powers[PowerIndex].cooldown;

        if(cd > 0) {
            textCD.gameObject.SetActive(true);
            ApplyCoolDown(cd);
        } else if (cd == 0) {
            textCD.gameObject.SetActive(false);
        }
    }

    public void ApplyCoolDown(float cd)
    {
        textCD.text = Mathf.RoundToInt(cd).ToString();
        imageCD.fillAmount = cd / maxDuration;
        print(maxDuration);
    }
}
