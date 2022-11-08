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
        return false;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
