using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static EnemyInfo;

public class CivilianMovement : AEnemyMovement
{
    void Start() 
    {

    }

    void Update()
    {
        Move();
        AllowedMovement();
    }
    public override void BasicMovement()
    {
        speed = EnemySpeed[EnemyType.Random];
    }

    public override void AlertMovement()
    {
        speed = 0f;
    }

    public override void SpotMovement()
    {
        speed = EnemySpeed[EnemyType.Random] + 2f;
    }
}
