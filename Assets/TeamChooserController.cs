using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamChooserController : MonoBehaviour
{
    public TeamBuilder AvailableEntities; 

    [SerializeField]
    private OptionsArrayHolder PilotArrayHolder;
    [SerializeField]
    private OptionsArrayHolder MechArrayHolder;
    public GameObject imagePrefab;

    public Text teamSizeText;
    private MapMarkerController.MapLevel givenLevel;
    private int currentTeamSpotIndex = 0;
    private List<TeamSpot> currentTeamList = new List<TeamSpot>();





    // To choose a team for a level we need info, this is the entrance to this script
    public void AccessLevelBasedOnData(MapMarkerController.MapLevel levelInfo) {
        givenLevel = levelInfo;
        teamSizeText.text = "" + (currentTeamSpotIndex+1).ToString() + " / " + givenLevel.teamMemberMax.ToString();

        PilotArrayHolder.CreateMechPortaits(AvailableEntities, imagePrefab);
        MechArrayHolder.CreateMechPortaits(AvailableEntities, imagePrefab);
    }

    [System.Serializable]
    public class OptionsArrayHolder {
        public Transform objectToHoldOptions;
        public bool isForPilots = true;

        public void CleanUpArrayHolder() {
            // Iterate through all children and destroy them
            for (int i = objectToHoldOptions.childCount - 1; i >= 0; i--) {
                Object.Destroy(objectToHoldOptions.GetChild(i).gameObject);
            }
        }

        public void CreateMechPortaits(TeamBuilder AvailableEntities, GameObject imagePrefab) {
            CleanUpArrayHolder();
            int listLength = AvailableEntities.PilotLength();
            if (!isForPilots) {
                listLength = AvailableEntities.MechLength();
            }

            for (int i = 0; i < listLength; i++) {
                // Instantiate the prefab at the specified position and rotation
                GameObject instantiatedObject = Instantiate(imagePrefab, objectToHoldOptions.position, objectToHoldOptions.rotation, objectToHoldOptions);
            }
        }
    }

    [System.Serializable]
    public class TeamSpot {
        public MechStats chosenMech;
        public CharacterStats chosenPilot;
    }
}