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
    public bool IsFlickingFreeze => Time.time < lastFlickTime + flickFreezeTime;

    [Header("Collisions")]
    public float flickRadius = 0.7f;
    public LayerMask layerMask;

    public static Vector3 flickDirection { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        inputMap = new InputMap();
        inputMap.Gameplay.Enable();
        inputMap.Gameplay.Flick.performed += Flick_performed;

        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        flickDirection = GetFlickDirection();

    }

    private void Flick_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!IsFlicking)
            Flick();
    }

    void Flick()
    {
        anim.SetTrigger("Flick");

        lastFlickTime = Time.time;

        List<Creature> creatures = GetHitCreatures();
        foreach (Creature creature in creatures)
        {
            creature.Hit(flickDirection);
            ScoreSystem.Instance.RegisterComboStarter(creature);
        }
    }

    List<Creature> GetHitCreatures()
    {
        List<Creature> creatures = new();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, flickRadius, layerMask);
        foreach (var col in colliders)
        {
           if(col.TryGetComponent(out Creature creature))
                creatures.Add(creature);
        }
        return creatures;
    }

    Vector3 GetFlickDirection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane plane = new Plane(-Vector3.forward, Vector3.zero);
        Vector3 raycastPoint;
        if (plane.Raycast(ray, out float distance))
        {
            raycastPoint = ray.origin + ray.direction * distance;

            Vector3 dir = raycastPoint - transform.position;
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
