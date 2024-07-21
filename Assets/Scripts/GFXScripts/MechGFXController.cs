using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechGFXController : MonoBehaviour
{
    public float yOffset;
    
    private void Start() {
        Vector3 currentPos = transform.position;
        transform.position = new Vector3(currentPos.x, currentPos.y + yOffset, currentPos.z);
    }
}
