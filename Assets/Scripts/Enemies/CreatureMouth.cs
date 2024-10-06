using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureMouth : Creature
{
    public GameObject attackFX;
    public SpriteRenderer attackFXSprite;
    Camera cam;

    public float minViewportDistance;
    public float maxViewportDistance;

    float viewportAttackDistance;

    bool isAttacking;

    protected override void Awake()
    {
        base.Awake();

        viewportAttackDistance = Random.Range(minViewportDistance, maxViewportDistance);
        cam = Camera.main;
    }

    public override void Init(EnemySpawner spawner)
    {
        base.Init(spawner);

        isAttacking = false;
        anim.SetBool("IsAttacking", false);
        StopAllCoroutines();
        attackFX.gameObject.SetActive(false);
    }

    public override void Hit(Vector3 dir)
    {
        base.Hit(dir);

        attackFX.gameObject.SetActive(false);
        StopAllCoroutines();
        isAttacking = false;
        anim.SetBool("IsAttacking", false);
    }

    protected override void CollideWithObjective(Vector3 collisionPoint)
    {
        return;
    }

    void Update()
    {
        if (!GameManager.Instance.GameIsPlaying)
            return;

        if(!isAttacking && !isHit)
        {
            isAttacking = IsInAttackDistance();
            if (isAttacking)
            {
                anim.SetBool("IsAttacking", true);
                StartCoroutine(AttackLoop());
            }
        }

        if ((isHit || isAttacking) && !isFlying)
            body.velocity = Vector3.zero;
        else
            MoveTowardObjective();
    }

    bool IsInAttackDistance()
    {
        Vector3 viewportPos = cam.WorldToViewportPoint(transform.position);
        viewportPos.z = 0;
        float dist = Vector3.Distance(viewportPos, new Vector3(0.5f, 0.5f, 0));

        return dist < viewportAttackDistance;
    }

    const float HEIGHT = 1.28f;
    IEnumerator AttackLoop()
    {
        while(true)
        {
            yield return new WaitForSeconds(2);

            attackFX.gameObject.SetActive(true);
            attackFX.transform.LookAt(Vector3.zero, -Vector3.forward);
            attackFXSprite.size = new Vector2(Vector3.Distance(attackFX.transform.position, Vector3.zero) / attackFXSprite.transform.localScale.x, HEIGHT);
            yield return new WaitForSeconds(0.15f);

            Vector3 dir = attackFX.transform.position;
            dir.Normalize();
            Cheese.Instance.DamageCheese(dir * 0.4f);

            yield return new WaitForSeconds(0.35f);
            attackFX.gameObject.SetActive(false);
        }
    }
}
