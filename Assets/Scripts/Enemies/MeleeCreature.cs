using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeCreature : Creature
{
    protected override void CollideWithObjective(Vector3 collisionPoint)
    {
        if (!GameManager.Instance.GameIsPlaying)
            return;

        if (isFlying)
            return;


        Cheese.Instance.DamageCheese(collisionPoint);

        Disable();
    }

    void Update()
    {
        if (!GameManager.Instance.GameIsPlaying)
            return;

        if (isHit && !isFlying)
            body.velocity = Vector3.zero;
        else
            MoveTowardObjective();
    }
}
