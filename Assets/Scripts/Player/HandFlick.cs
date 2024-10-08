using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandFlick : MonoBehaviour
{
    InputMap inputMap;
    Animator anim;

    [Header("Cooldown")]
    float lastFlickTime;
    public float flickCooldown = 0.3f;
    public float flickFreezeTime = 0.3f;
    public bool IsFlicking => Time.time < lastFlickTime + flickCooldown;
    public bool IsFlickingFreeze => Time.time < lastFlickTime + flickFreezeTime || isHurt;

    [Header("Hurt")]
    bool isHurt;
    const float HURT_DURATION = 0.5f;

    [Header("Collisions")]
    public float flickRadius = 0.7f;
    public LayerMask layerMask;

    public static HandFlick Instance;
    public static Vector3 flickDirection { get; private set; }
    public static Vector3 cursorPos { get; private set; }

    //Hover
    Creature hoveredCreature;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        inputMap = new InputMap();
        inputMap.Gameplay.Enable();
        inputMap.Gameplay.Flick.performed += Flick_performed;

        anim = GetComponentInChildren<Animator>();
    }

    private void OnDestroy()
    {
        inputMap.Gameplay.Flick.performed -= Flick_performed;
        inputMap.Dispose();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.gameIsOver)
            return;
        flickDirection = GetFlickDirection();

        Creature closestCreature = GetClosestTarget();
        if (closestCreature != hoveredCreature)
        {
            if (hoveredCreature != null)
                hoveredCreature.OnFlickHoverExit();

            hoveredCreature = closestCreature;
            if (hoveredCreature != null)
            {
                hoveredCreature.OnFlickHoverEnter();
            }
        }
    }

    private void Flick_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (GameManager.Instance.gameIsOver)
            return;

        if (!IsFlicking)
            Flick();
    }

    void Flick()
    {
        anim.SetTrigger("Flick");
        SFXManager.PlaySound(GlobalSFX.Flick);

        lastFlickTime = Time.time;

        Creature closestCreature = GetClosestTarget();
        if (closestCreature != null)
        {
            if (closestCreature.CanBeHit())
            {
                closestCreature.Hit(flickDirection);
                ScoreSystem.Instance.RegisterComboStarter(closestCreature);
            }
            //Hit by hedgehog
            else
            {
                anim.SetBool("IsHurt", true);
                isHurt = true;
                Invoke("ClearHurt", HURT_DURATION);
                SFXManager.PlaySound(GlobalSFX.FlickHurt);
                ScreenShakeSimple.Instance.Shake(0.6f);
            }
        }
    }

    Creature GetClosestTarget()
    {
        List<Creature> creatures = GetHitCreatures();
        Creature closestCreature = null;
        float minDist = Mathf.Infinity;
        foreach (Creature creature in creatures)
        {
            if (creature.isHit)
                continue;
            float dist = Vector3.Distance(transform.position, creature.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closestCreature = creature;
            }
        }
        return closestCreature;
    }

    void ClearHurt()
    {
        isHurt = false;
        anim.SetBool("IsHurt", false);
    }

    List<Creature> GetHitCreatures()
    {
        List<Creature> creatures = new();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, flickRadius, layerMask);
        foreach (var col in colliders)
        {
            if (col.TryGetComponent(out Creature creature))
                creatures.Add(creature);
        }
        return creatures;
    }

    Vector3 GetFlickDirection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane plane = new Plane(-Vector3.forward, Vector3.zero);
        if (plane.Raycast(ray, out float distance))
        {
            cursorPos = ray.origin + ray.direction * distance;

            Vector3 dir = cursorPos - transform.position;
            dir.z = 0;
            dir.Normalize();
            return dir;
        }
        Debug.LogError("Cursor isn't hover place");
        return Vector3.zero;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (!IsFlicking)
            Gizmos.color = Color.blue;
        else
            Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, flickRadius);

        Gizmos.DrawRay(transform.position, GetFlickDirection());
    }
#endif
}
