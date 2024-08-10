using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCameraController : MonoBehaviour
{
    public float moveSpeed = 7f;
    private CameraController camController;

    private void Start() {
        camController =  gameObject.AddComponent<CameraController>();
        camController.SetSpeed(moveSpeed);
    }
}
