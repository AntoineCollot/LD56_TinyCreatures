using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandMovement : MonoBehaviour
{
    InputMap inputMap;
    public float moveSpeed;
    HandFlick flicker;
    float flickerModifier;
    float refFlickerModifier;

    const float FLICKER_SMOOTH = 0.1f;

    void Start()
    {
        inputMap = new InputMap();
        inputMap.Gameplay.Enable();
        flicker = GetComponent<HandFlick>();
    }


    private void OnDestroy()
    {
        inputMap.Dispose();
    }

    void Update()
    {
        if (GameManager.Instance.gameIsOver)
            return;

        Vector2 playerInputs = inputMap.Gameplay.Move.ReadValue<Vector2>();
        Vector3 move = playerInputs;

        //Flicker stop
        float flickerTarget = 1;
        if (flicker.IsFlickingFreeze)
            flickerTarget = 0;
        flickerModifier = Mathf.SmoothDamp(flickerModifier, flickerTarget, ref refFlickerModifier, FLICKER_SMOOTH);
        float effetctiveMoveSpeed = moveSpeed * flickerModifier;

        move *= effetctiveMoveSpeed * Time.deltaTime;
        transform.Translate(move, Space.World);

    }
}
