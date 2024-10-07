using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cheese : MonoBehaviour
{
    public int hp = 5;
    public AnimationCurve hitAnim;
    public GameObject fx;

    [Header("Sprite")]
    public Sprite[] cheeseSprites;
    SpriteRenderer sprite;

    public static Cheese Instance;

    void Awake()
    {
        Instance = this;

        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    public void DamageCheese(Vector3 point)
    {
        if (fx != null)
        {
            Destroy(Instantiate(fx, point, Quaternion.identity, null), 1);
        }

        hp--;
        StopAllCoroutines();
        StartCoroutine(HitAnim());
        Invoke("UpdateSprite", 0.1f);
        ScreenShakeSimple.Instance.Shake(1f);
        if (hp <= 0)
        {
            GameManager.Instance.GameOver();
        }
    }

    void UpdateSprite()
    {
        if (hp > 0 && hp < cheeseSprites.Length)
        {
            sprite.sprite = cheeseSprites[hp];
        }
        else if (hp <= 0)
            sprite.sprite = cheeseSprites[0];

    }

    IEnumerator HitAnim()
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / 0.3f;

            transform.localScale = Vector3.one * hitAnim.Evaluate(t);

            yield return null;
        }
    }
}
