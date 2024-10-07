using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLine : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    float height;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        height = spriteRenderer.size.y;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float width = Vector3.Distance(transform.position, HandFlick.cursorPos);
        spriteRenderer.size = new Vector2(width, height);
    }
}
