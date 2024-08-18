using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapSelectorController : MonoBehaviour
{
    public enum MapState {
        ChoosingLevelFromMap,
        PickingTeamOnChosenLevel
    }
    public MapState mapState =  MapState.ChoosingLevelFromMap;
    public MapMarkerController currentMarker;

    [Header("Cursor Variables")]
    public Transform mapCursor;
    public float cursorYOffset = 0.2f;
    public float selectionAimAssistRange = 2f;
    public bool uses3DCursor = false;


    [Header("Dissolve Variables")]
    public List<Material> dissolveMats = new List<Material>();
    public float dissolveSpeed = 1.0f;
    public GameObject MapUIGroup;
    public GameObject UpperLevelGroup;
    private bool DoDissolve = false;

    [Header("TeamChooser Object Variables")]
    public GameObject lowerTeamChooserObject;
    public float yLowerOffset = 1f;



    private CameraController camScript;
    private Camera mainCamera;
    private List<MapMarkerController> mapMarkers = new List<MapMarkerController>();
    // Start is called before the first frame update
    private void Start()
    {
        camScript = FindObjectOfType<CameraController>();
        mainCamera = Camera.main;
        MapMarkerController[] markersArray = FindObjectsOfType<MapMarkerController>();
        foreach (MapMarkerController marker in markersArray)
        {
            mapMarkers.Add(marker);
        }

        MaterialSetup();

        if (!uses3DCursor) {
            mapCursor.gameObject.SetActive(false);
        }
    }


    void Update()
    {
        Vector3 camLookPos = CollectCursorMoveInput();

        if (uses3DCursor) {
            SetCursorPos(camLookPos);
        }
        SortMarkerActivity(camLookPos);
        ControlMaterialDissolve(DoDissolve);

        if (mapState == MapState.ChoosingLevelFromMap) {
            if (camLookPos != Vector3.zero) // Only update if we have a valid look position
            {
                // Debug.Log("IN ChoosingLevelFromMap state");
                SelectMap();
            }
        } 

        if (Input.GetKeyDown(KeyCode.Escape)) {
            // Debug.Log("escape key pressed");
            currentMarker = null;
            mapState = MapState.ChoosingLevelFromMap;
            DoDissolve = false;
        }
        


    }

    private void SelectMap() {
        if (Input.GetMouseButtonDown(0)) {
            //  Debug.Log("Actiavted SelectMap method");
            if (currentMarker != null) {
                DoDissolve = true;
                Vector3 levelPosition = currentMarker.gameObject.transform.position;
                GiveLevelToTeamChooser();
                currentMarker = null;
                lowerTeamChooserObject.transform.position = new Vector3(levelPosition.x, levelPosition.y - yLowerOffset, levelPosition.z);


            }
        }
    }

    private void GiveLevelToTeamChooser() {
        MapMarkerController.MapLevel levelChosen = currentMarker.GiveMarkerLevel();
        TeamChooserController teamChoosingScript = lowerTeamChooserObject.GetComponent<TeamChooserController>();
        teamChoosingScript.AccessLevelBasedOnData(levelChosen);

    }

    private void MaterialSetup() {
        foreach (Material materialDissolve in dissolveMats)
        {
            materialDissolve.SetFloat("_DissolvePower", 0f);
        }
    }

    // Material Methods
    private void ControlMaterialDissolve(bool dissolve)
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
            if (newValue == 1f) {
                UpperLevelGroup.SetActive(false);
            }
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

    private Vector3 CollectCursorMoveInput() {

        return CollectMousePositionInWorld();
        // return CollectCameraLookingPosition();

    }

    private Vector3 CollectMousePositionInWorld() {
        // Get the mouse position on the screen
        Vector3 mousePosition = Input.mousePosition;

        // Create a ray from the camera through the mouse position
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);

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
    private Vector3 CollectCameraLookingPosition() {
        // Create a ray from the camera's position in the direction it is facing
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);

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
