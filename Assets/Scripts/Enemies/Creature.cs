using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Creature : MonoBehaviour
{
    public EnemySpawnData.Type type;
    protected Animator anim;
    protected Collider2D col;
    protected Rigidbody2D body;
    protected int moveHash;
    protected EnemySpawner spawner;
    public float maxMoveSpeed = 2;
    public float MoveSpeed => maxMoveSpeed * GameDifficulty.MovementMultiplier;

    protected bool isHit;
    protected bool isFlying;
    protected Vector2 hitDirection;
    public float projectionSpeed = 8;
    public float projectionRotation = 400;

    protected virtual void Awake()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        body = GetComponent<Rigidbody2D>();
        moveHash = Animator.StringToHash("MoveSpeed");
    }

    protected virtual void Start()
    {
        GameManager.Instance.onGameOver.AddListener(OnGameOver);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.onGameOver.RemoveListener(OnGameOver);
    }

    protected virtual void OnGameOver()
    {
        body.velocity = Vector2.zero;
        body.angularVelocity = 0;
    }

    public virtual void Init(EnemySpawner spawner)
    {
        this.spawner = spawner;
        isHit = false;
        isFlying = false;
        col.enabled = true;
        body.freezeRotation = true;
        body.velocity = Vector2.zero;
        body.angularVelocity = 0;
        anim.SetBool("IsFlying", false);
        anim.ResetTrigger("Hit");
        transform.rotation = Quaternion.identity;
        CancelInvoke();

        ScoreSystem.Instance.ClearCreatureCombo(this);
    }

    public virtual bool CanBeHit()
    {
        return true;
    }

    public virtual void Hit(Vector3 dir)
    {
        if (isHit)
            return;
        isHit = true;
        spawner.RegisterHit(this);
        anim.SetTrigger("Hit");
        hitDirection = dir.normalized;
        ScoreSystem.Instance.AddHitPoints();

        Invoke("FlyDelayed", 0.1f);
    }

    protected virtual void FlyDelayed()
    {
        anim.SetBool("IsFlying", true);
        body.freezeRotation = false;
        isFlying = true;
        body.velocity = hitDirection * projectionSpeed;
        body.angularVelocity = projectionRotation;
    }

    public virtual void Disable()
    {
        gameObject.SetActive(false);
        spawner.AddToPool(this);
    }

    public virtual void MoveTowardObjective()
    {
        if (isHit || !GameManager.Instance.GameIsPlaying)
            return;
        Vector2 dir = -transform.position;
        dir.Normalize();
        body.velocity = dir * MoveSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Vector3 hitPos = (other.transform.position + transform.position) * 0.5f;
        if (other.CompareTag("Objective"))
            CollideWithObjective(hitPos);

        string tag = other.tag;
        if (isFlying && other.CompareTag("Creature"))
        {
            if (other.TryGetComponent(out Creature creature))
            {
                if (creature.isHit)
                    return;

                if (!creature.CanBeHit())
                    return;

                //Register combo
                ScoreSystem.Instance.RegisterCombo(this, creature, hitPos);

                Vector3 hitDir = creature.transform.position - transform.position;
                creature.Hit(hitDir.normalized);
            }
        }
    }

    protected abstract void CollideWithObjective(Vector3 collisionPoint);
}
