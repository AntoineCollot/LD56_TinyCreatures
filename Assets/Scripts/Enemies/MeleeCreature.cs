using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeCreature : Creature
{
    public GameObject fx;

    protected override void CollideWithObjective(Vector3 collisionPoint)
    {
        if (isFlying)
            return;

        if (fx != null)
        {
            Instantiate(fx, collisionPoint, Quaternion.identity, null);
        }
        Disable();
    }

    void Update()
    {
        if (isHit && !isFlying)
            body.velocity = Vector3.zero;
        else
            MoveTowardObjective();
    }
}
