using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamChooserController : MonoBehaviour
{
    public string levelChosen;
    public TeamBuilder AvailableEntities; 

    public GameObject PilotArrayHolder;
    public GameObject MechArrayHolder;

    public GameObject imagePrefab;

    // Default position and rotation for instantiation
    private Vector3 instantiatePosition = Vector3.zero;
    private Quaternion instantiateRotation = Quaternion.identity;

    public void SetLevelStringReference(string stringLevelName) {
        levelChosen = stringLevelName;
    }

    private void Start() {
        Cleanup(MechArrayHolder.transform);
        CreateMechPortaits();
    }

    private void CreateMechPortaits() {
        for (int i = 0; i < AvailableEntities.MechLength(); i++) {
            // Instantiate the prefab at the specified position and rotation
            GameObject instantiatedObject = Instantiate(imagePrefab, instantiatePosition, instantiateRotation, MechArrayHolder.transform);

            // Optionally, reset local position and rotation if you want it to be relative to the parent
            instantiatedObject.transform.localPosition = Vector3.zero;
            instantiatedObject.transform.localRotation = Quaternion.identity;
        }
    }

    private void Cleanup(Transform target) {
        // Iterate through all children and destroy them
        for (int i = target.childCount - 1; i >= 0; i--) {
            Destroy(target.GetChild(i).gameObject);
        }
    }
}
