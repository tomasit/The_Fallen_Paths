using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GUI_Cooldown : MonoBehaviour
{
    public int PowerIdex;

    [SerializeField] private Image imageCD;
    [SerializeField] private TMP_Text textCD;
    private PowerManager powerManager;
    private float cd = 0;
    private float maxDuration = 0;

    private void Start()
    {
        powerManager = FindObjectOfType<PowerManager>();
        textCD.gameObject.SetActive(false);
        maxDuration= powerManager._powers[PowerIdex].cooldownDuration;
    }

    private void Update()
    {
        cd = powerManager._powers[PowerIdex].cooldown;
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
