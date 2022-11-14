using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkEnemies : AVisualCircleRangedPower
{
    bool firstEnemySelected = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    public override void Cancel()
    {
        throw new System.NotImplementedException();
    }

    public override void Fire()
    {
        throw new System.NotImplementedException();
    }

    protected override void Preview()
    {
        throw new System.NotImplementedException();
    }

    protected override void UnPreview()
    {
        throw new System.NotImplementedException();
    }

    protected override bool canCastPower()
    {
        if (!firstEnemySelected)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//Define the ray pointed by the mouse in the game window
            RaycastHit hitInfo; //Information of ray collision
            if (Physics.Raycast(ray, out hitInfo))
                Debug.Log(hitInfo.collider.gameObject.tag);//Determine whether to hit the object            return false;
            Debug.Log(hitInfo);
            return false;
        }
        else
            return true;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
