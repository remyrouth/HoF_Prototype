using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSelectorController : MonoBehaviour
{
    public MapMarkerController currentMarker;

    [Header("Cursor Variables")]
    public Transform mapCursor;
    public float cursorYOffset = 0.2f;
    public float selectionAimAssistRange = 2f;


    [Header("Dissolve Variables")]
    public List<Material> dissolveMats = new List<Material>();
    public float dissolveSpeed = 1.0f;
    public GameObject MapUIGroup;
    public GameObject UpperLevelGroup;
    private bool DoDissolve = false;



    private CameraController cam;
    private List<MapMarkerController> mapMarkers = new List<MapMarkerController>();
    // Start is called before the first frame update
    private void Start()
    {
        cam = FindObjectOfType<CameraController>();
        MapMarkerController[] markersArray = FindObjectsOfType<MapMarkerController>();
        foreach (MapMarkerController marker in markersArray)
        {
            mapMarkers.Add(marker);
        }

        MaterialSetup();
    }


    void Update()
    {
        Vector3 camLookPos = CameraLook();
        if (camLookPos != Vector3.zero) // Only update if we have a valid look position
        {

            SetCursorPos(camLookPos);
            SortMarkerActivity(camLookPos);
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
           DoDissolve = !DoDissolve;
        //    Debug.Log("DoDissolve: " + DoDissolve);
        }

        ToggleDissolveMaterials(DoDissolve);

    }

    private void MaterialSetup() {
        foreach (Material materialDissolve in dissolveMats)
        {
            materialDissolve.SetFloat("_DissolvePower", 0f);
        }
    }

    // Material Methods
    private void ToggleDissolveMaterials(bool dissolve)
    {
        // Debug.Log("DoDissolve: " + DoDissolve);
        MapUIGroup.SetActive(!dissolve);
        foreach (Material materialDissolve in dissolveMats)
        {
            ChangeDissolve(materialDissolve, dissolve);
        }
    }

    private void ChangeDissolve(Material material, bool dissolve)
    {
        float floatValue = material.GetFloat("_DissolvePower");
        float newValue = 0f;
        if (dissolve) {
            newValue = floatValue + (dissolveSpeed * Time.deltaTime);
            newValue = Mathf.Min(newValue, 1f);
            UpperLevelGroup.SetActive(false);
        } else {
            newValue = floatValue - (dissolveSpeed * Time.deltaTime);
            newValue = Mathf.Max(newValue, 0f);
            UpperLevelGroup.SetActive(true);
            // if (newValue == 0f) {

            // }
        }


        material.SetFloat("_DissolvePower", newValue);
    }



    // UI Methods
    private void SortMarkerActivity(Vector3 camLookPos) {
        MapMarkerController newMarker = ClosestMarkerFinder(camLookPos);
        if (newMarker != currentMarker) {
            if (currentMarker != null) {
                currentMarker.ActivateNeutral();
            }
            currentMarker = newMarker;
            if (currentMarker != null) {
                currentMarker.ActivateSelected();
            }
        }
    }

    private void SetCursorPos(Vector3 newPos) {
        mapCursor.position = new Vector3(newPos.x, newPos.y + cursorYOffset, newPos.z);
    }

    private MapMarkerController ClosestMarkerFinder(Vector3 lookPos) {
        MapMarkerController closestMarker = null;
        float closestDistance = float.MaxValue;
        foreach (MapMarkerController marker in mapMarkers)
        {
            float distance = Vector3.Distance(lookPos, marker.transform.position);
            if (distance < closestDistance && distance <= selectionAimAssistRange)
            {
                closestDistance = distance;
                closestMarker = marker;
            }
        }
        
        // Only check distance if a marker was found
        if (closestMarker != null && Vector3.Distance(lookPos, closestMarker.transform.position) <= selectionAimAssistRange) {
            return closestMarker;
        } else {
            return null;
        }
}

    private Vector3 CameraLook() {
        // Create a ray from the camera's position in the direction it is facing
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

        // Store the hit information
        RaycastHit hit;

        // Perform the raycast
        float maxDistance = 50f;
        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            // Optional: Draw a debug line in the scene view
            Debug.DrawLine(ray.origin, hit.point, Color.red);

            // Optional: Visualize the point with a sphere in the scene view
            Debug.DrawLine(hit.point, hit.point + Vector3.up * 0.1f, Color.green);
            Debug.DrawLine(hit.point, hit.point - Vector3.up * 0.1f, Color.green);
            Debug.DrawLine(hit.point, hit.point + Vector3.right * 0.1f, Color.green);
            Debug.DrawLine(hit.point, hit.point - Vector3.right * 0.1f, Color.green);
            Debug.DrawLine(hit.point, hit.point + Vector3.forward * 0.1f, Color.green);
            Debug.DrawLine(hit.point, hit.point - Vector3.forward * 0.1f, Color.green);

            return hit.point;
        } else {
            return Vector3.zero;
        }

    }
}
