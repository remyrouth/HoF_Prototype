using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamChooserUI : MonoBehaviour
{
    [SerializeField] private Text teamSizeText;
    [SerializeField] private OptionsArrayHolder pilotArrayHolder;
    [SerializeField] private OptionsArrayHolder mechArrayHolder;
    [SerializeField] private GameObject spriteOptionHolderPrefab;

    private TeamModel teamModel;
    private TeamBuilder totalAvailableEntities;

    public void Initialize(TeamModel model, TeamBuilder newEntityList)
    {
        totalAvailableEntities = newEntityList;
        teamModel = model;
        pilotArrayHolder.CreateEntityPortaits(totalAvailableEntities, spriteOptionHolderPrefab, teamModel);
        mechArrayHolder.CreateEntityPortaits(totalAvailableEntities, spriteOptionHolderPrefab, teamModel);
    }

    public void UpdateUI() {
        UpdateTeamSpotText();
    }

    private void UpdateTeamSpotText() {
        int currentTeamSpotIndex = teamModel.CurrentSpotIndex;
        int teamMemberMax = teamModel.TeamSpots.Count;
        teamSizeText.text = "" + (currentTeamSpotIndex+1).ToString() + " / " + teamMemberMax.ToString();
    }

    // Methods to handle UI interactions
    public void OnTeamSpotChangeClicked(bool increase) {
        teamModel.ChangeCurrentSpot(increase);
        UpdateTeamSpotText();
    }


    [System.Serializable]
    public class OptionsArrayHolder {
        public Transform objectToHoldOptions;
        public bool isForPilots = true;

        public void CleanUpArrayHolder() {
            // Iterate through all children and destroy them
            for (int i = objectToHoldOptions.childCount - 1; i >= 0; i--) {
                // Object.Destroy(objectToHoldOptions.GetChild(i).gameObject);
                UnityEngine.Object.Destroy(objectToHoldOptions.GetChild(i).gameObject);
            }
        }

        public void CreateEntityPortaits(TeamBuilder AvailableEntities, GameObject spriteOptionHolderPrefab, TeamModel teamModel) {
            CleanUpArrayHolder();
            int listLength = AvailableEntities.PilotLength();
            if (!isForPilots) {
                listLength = AvailableEntities.MechLength();
            }

            for (int i = 0; i < listLength; i++) {
                GameObject instantiatedObject = Instantiate(spriteOptionHolderPrefab, objectToHoldOptions.position, objectToHoldOptions.rotation, objectToHoldOptions);
                TeamSpotOptionController spotOptionObject = instantiatedObject.GetComponent<TeamSpotOptionController>();
                
                if (isForPilots) {
                    spotOptionObject.BecomePilotOption(AvailableEntities.GetPilot(i), teamModel);
                } else {
                    spotOptionObject.BecomeMechOption(AvailableEntities.GetMech(i), teamModel);
                }
            }
        }
    }

}
