using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipSpriteByFlickDir : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    float xOffset;

    // Start is called before the first frame update
    void Start()
    {
        xOffset = transform.localPosition.x;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        bool flip = HandFlick.flickDirection.x < 0;
        Vector3 localPos = transform.localPosition;
        if(flip)
            localPos.x = -xOffset;
        else
            localPos.x = -xOffset;

        transform.localPosition = localPos;
        spriteRenderer.flipX = flip;
    }
}
