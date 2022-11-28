using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCoroutine1 : ACoroutine
{
    private void Start()
    {
    }

    //Run enemy
    public override IEnumerator Interact(Transform obj = null)//obj = enemy to target
    {
        yield return new WaitForSeconds(1);
        Debug.Log("Coroutine 1 computed");
    }
}
