using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PowerManager : MonoBehaviour
{
    [SerializeField] private APower[] _powers;

    private void Start()
    {
        foreach (var power in _powers)
        {
            if (power is ARangedPower) {
                //Debug.Log("range power");
            } else {
                //Debug.Log("usual power");
            }
        }
    } 
}
