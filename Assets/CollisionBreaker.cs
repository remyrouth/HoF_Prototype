using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionBreaker : MonoBehaviour
{
    [SerializeField] private GameObject OriginalObject; // object that should look like its breaking apart
    [SerializeField] private List<GameObject> breakParts = new List<GameObject>();
    [SerializeField] private float delay = 3f;
    [SerializeField] private Vector3 breakDirection = Vector3.up;
    [SerializeField] private float forceMagnitude = 5f;
    [SerializeField] private float randomizationDegree = 30f;
    [SerializeField] private Vector3 breakPoint;
    [SerializeField] private float deleteTimer = 10f;

    private void Start()
    {
        foreach (GameObject part in breakParts)
        {
            part.SetActive(false);
        }
        Invoke("Break", delay);
        Invoke("SafetyDelete", deleteTimer);
    }

    private void SafetyDelete() {
        foreach (GameObject part in breakParts)
        {
            Destroy(part);
        }
    }

    private void Break()
    {
        if (OriginalObject) {
            Destroy(OriginalObject);
        }

        foreach (GameObject part in breakParts)
        {
            part.SetActive(true);
            part.transform.SetParent(null);
            Rigidbody rb = part.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = part.AddComponent<Rigidbody>();
            }

            Vector3 randomizedDirection = RandomizeDirection(breakDirection, randomizationDegree);
            Vector3 forceDirection = (part.transform.position - breakPoint).normalized;
            Vector3 finalForce = Vector3.Lerp(randomizedDirection, forceDirection, 0.5f).normalized * forceMagnitude;

            rb.AddForce(finalForce, ForceMode.Impulse);
            rb.AddTorque(Random.insideUnitSphere * forceMagnitude, ForceMode.Impulse);
        }
    }

    private Vector3 RandomizeDirection(Vector3 direction, float maxAngle)
    {
        return Quaternion.AngleAxis(Random.Range(-maxAngle, maxAngle), Random.insideUnitSphere) * direction;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(breakPoint, 0.1f);
        Gizmos.DrawRay(breakPoint, breakDirection);

        if (Application.isPlaying)
        {
            Gizmos.color = Color.green;
            foreach (GameObject part in breakParts)
            {
                if (part != null)
                {
                    Gizmos.DrawLine(breakPoint, part.transform.position);
                }
            }
        }
    }
}
