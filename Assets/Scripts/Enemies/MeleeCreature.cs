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

        SFXManager.PlaySound(GlobalSFX.MouseEat);
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

    public override void Hit(Vector3 dir)
    {
        base.Hit(dir);

        SFXManager.PlaySound(GlobalSFX.MouseHit);
    }
}
