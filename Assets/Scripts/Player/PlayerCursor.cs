using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCursor : MonoBehaviour
{
    Plane plane;
    public static Vector3 cursorPosition;

    // Start is called before the first frame update
    void Start()
    {
        plane = new Plane(-Vector3.forward,Vector3.zero);
    }

    // Update is called once per frame
    void Update()
    {
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        plane.Raycast(camRay, out float distance);

        cursorPosition = camRay.origin + camRay.direction * distance;
    }
}
