using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformCollapser : MonoBehaviour
{
    [SerializeField] private List<GameObject> fallingPlatforms = new List<GameObject>();
    [SerializeField] private List<BridgeObject> BridgePlatforms = new List<BridgeObject>();
    [SerializeField] private AnimationCurve fallCurve;
    [SerializeField] private float fallDuration = 0.25f;

    public void Start() {
    }

    private void OnTriggerEnter(Collider other) {
        Collider triggerCollider = GetComponent<Collider>();
        if (triggerCollider != null) {
            triggerCollider.enabled = false;
        }
        StartBridgeRotations();
    }

    private void StartCollapse() {
        foreach(GameObject platformObject in fallingPlatforms) {
            platformObject.AddComponent<Rigidbody>();
        }
    }

    private void StartBridgeRotations() {
        foreach(BridgeObject platform in BridgePlatforms) {
            StartCoroutine(RotateXCoroutine(platform.platform, platform.endRotation, fallDuration, fallCurve));
        }
    }

    private IEnumerator RotateXCoroutine(GameObject targetObject, float targetRotation, float duration, AnimationCurve curve) {
        Quaternion startRotation = targetObject.transform.rotation;
        Quaternion endRotation = Quaternion.Euler(targetRotation, 0f, 90f);
        float elapsedTime = 0f;

        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(elapsedTime / duration);
            float curveValue = curve.Evaluate(normalizedTime);
            targetObject.transform.rotation = Quaternion.Slerp(startRotation, endRotation, curveValue);
            yield return null;
        }

        targetObject.transform.rotation = endRotation;
    }

    [System.Serializable]
    public class BridgeObject {
        public GameObject platform;
        public float endRotation;
    }
}