using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float radius;
    public LayerMask layerMask;

    public void Init(Creature source)
    {
        StartCoroutine(Explode(source));
    }

    IEnumerator Explode(Creature source)
    {
        SFXManager.PlaySound(GlobalSFX.Explosion);

        yield return new WaitForSeconds(0.15f);

        List<Creature> list = GetHitCreatures();
        foreach (Creature creature in list)
        {
            if (creature.isHit)
                continue;

            if (!creature.CanBeHit())
                continue;

            //Register combo
            ScoreSystem.Instance.RegisterCombo(source, creature, creature.transform.position);

            Vector3 hitDir = creature.transform.position - transform.position;
            creature.Hit(hitDir.normalized);
        }
        ScreenShakeSimple.Instance.Shake(0.7f);
        Destroy(gameObject, 0.5f);
    }

    List<Creature> GetHitCreatures()
    {
        List<Creature> creatures = new();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, layerMask);
        foreach (var col in colliders)
        {
            if (col.TryGetComponent(out Creature creature))
                creatures.Add(creature);
        }
        return creatures;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
#endif
}
