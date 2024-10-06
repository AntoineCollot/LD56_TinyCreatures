using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorPosIfFlipped : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    float xOffset;

    private void Start()
    {
        spriteRenderer = GetComponentInParent<SpriteRenderer>();
        xOffset = transform.localPosition.x;
    }

    private void Update()
    {
        Vector3 pos = transform.localPosition;

        if(spriteRenderer.flipX)
            pos.x = -xOffset;
        else
            pos.x = xOffset;

        transform.localPosition = pos;
    }
}
