using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// this is a script that allows players to choose a team before going on a mission
// it also is responsible for the UI of the team choosing phase
public class TeamChooserController : MonoBehaviour
{
    [SerializeField] private TeamChooserUI teamChooserUI;
    [SerializeField] private MechDisplayManager mechDisplayManager;
    [SerializeField] private TeamBuilder availableEntities;

    // we give this to the team roster persistor so that it can have a menu to activate 
    // in the combat scene
    [SerializeField] private GameObject unitPlacementControllerPrefab;

    private TeamModel teamModel;
    private MapMarkerController.MapLevel currentLevel;

    public void AccessLevelBasedOnData(MapMarkerController.MapLevel levelInfo)
    {
        Debug.Log("levelInfo max: " + levelInfo.teamMemberMax);
        if (teamModel != null) {
            teamModel = null;
        }
        teamModel = new TeamModel();
        currentLevel = levelInfo;
        teamModel.InitializeTeamList(currentLevel, mechDisplayManager);

        // availableEntities is just a scriptable object which 
        // holds all available entities we could ever choose.
        // In the future, we should replace this with entities 
        // from a text file
        teamChooserUI.Initialize(levelInfo, availableEntities);
        OnTeamChanged();
    }

    private void OnTeamChanged()
    {
        // MechStats currentMech = teamModel.TeamSpots[teamModel.CurrentSpotIndex].chosenMech;
        mechDisplayManager.DisplayMech(null);
        // MechDisplayManager
    }


    [System.Serializable]
    public class TeamSpot {
        public MechStats chosenMech;
        public CharacterStats chosenPilot;
    }




}