using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HedgehogCreature : Creature
{
    public float defendRadius;
    bool isDefending;

    protected override void CollideWithObjective(Vector3 collisionPoint)
    {
        if (!GameManager.Instance.GameIsPlaying)
            return;

        if (isFlying)
            return;

        SFXManager.PlaySound(GlobalSFX.HegEat);
        Cheese.Instance.DamageCheese(collisionPoint);

        Disable();
    }

    void Update()
    {
        if (!GameManager.Instance.GameIsPlaying)
            return;

        bool shouldDefend = ShouldDefend();
        if(shouldDefend && !isDefending)
        {
            SFXManager.PlaySound(GlobalSFX.HegDefend);
            anim.SetBool("IsDefending", true);
            isDefending = true;
        }
        else if(isDefending && !shouldDefend)
        {
            SFXManager.PlaySound(GlobalSFX.HegDefendOff);
            anim.SetBool("IsDefending", false);
            isDefending = false;
        }

        if ((isHit||isDefending) && !isFlying)
            body.velocity = Vector3.zero;
        else
            MoveTowardObjective();
    }

    public override bool CanBeHit()
    {
        return !isDefending;
    }

    bool ShouldDefend()
    {
        float distToFlick = Vector3.Distance(HandFlick.Instance.transform.position, transform.position);
        return distToFlick <= defendRadius;
    }

    public override void Hit(Vector3 dir)
    {
        base.Hit(dir);
        SFXManager.PlaySound(GlobalSFX.HegHit);
    }
}
