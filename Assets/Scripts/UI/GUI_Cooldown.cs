using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GUI_Cooldown : MonoBehaviour
{
    public float CDTime;
    private float Timer;
    [SerializeField]
    private Image imageCD;
    [SerializeField]
    private TMP_Text textCD;
    private bool IsCooldown = false;
    // private List<PowerManager.PowerData> powList = PowerManager._powers;

    private void Start()
    {
        textCD.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.P))
        {
            onCD();
        }
        if(IsCooldown)
        {
            ApplyCoolDown();
        }
    }
    private void ApplyCoolDown()
    {
        Timer  -= Time.deltaTime;

        if (Timer < 0.0f)
        {
            IsCooldown = false;
            textCD.gameObject.SetActive(false);
            imageCD.fillAmount = 0.0f;
        } else
        {
            textCD.text = Mathf.RoundToInt(Timer).ToString();
            imageCD.fillAmount = Timer / CDTime;
        }


    }
    public void onCD()
    {
        if (IsCooldown)
        {
            // Sound Effect or UI msg
        } else 
        {
            IsCooldown = true;
            Timer = CDTime;
            textCD.gameObject.SetActive(true);
            textCD.text = Timer.ToString();
        }
    }
}
