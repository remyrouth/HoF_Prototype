using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechGFXController : MonoBehaviour
{
    public float yOffset;
    
    private void Start() {
        Vector3 currentPos = transform.position;
        // transform.position = new Vector3(currentPos.x, currentPos.y + yOffset, currentPos.z);

        GameObject tileObject = GameObject.FindWithTag("Tile");
        if (tileObject != null) {
            Vector3 tileVector3Pos = tileObject.transform.position;
            transform.position = new Vector3(currentPos.x, tileVector3Pos.y + yOffset, currentPos.z);
        } else {
            Vector3 origin = transform.position;
            Vector3 direction = Vector3.down;
            if (Physics.Raycast(origin, direction, out RaycastHit hit))
            {
                float yPos = hit.point.y;
                transform.position = new Vector3(currentPos.x, yPos + yOffset, currentPos.z);
            }
            else
            {
                Debug.Log("The raycast did not hit any object.");
            }

        }

    }
}
