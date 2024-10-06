using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtFlickDir : MonoBehaviour
{
    // Update is called once per frame
    void LateUpdate()
    {
        transform.LookAt(transform.position + HandFlick.flickDirection,-Vector3.forward);
    }
}
