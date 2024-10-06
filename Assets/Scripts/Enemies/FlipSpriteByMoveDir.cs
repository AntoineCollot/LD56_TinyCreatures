using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipSpriteByMoveDir : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Vector3 lastPos;

    const float MIN_SPEED = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        lastPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 delta = lastPos - transform.position;
        if (delta.magnitude / Time.deltaTime < MIN_SPEED)
            return;
        spriteRenderer.flipX = delta.x < 0;

        lastPos = transform.position;
    }
}
