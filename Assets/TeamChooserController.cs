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

    private TeamModel teamModel;
    private MapMarkerController.MapLevel currentLevel;

    public void AccessLevelBasedOnData(MapMarkerController.MapLevel levelInfo)
    {
        if (teamModel != null) {
            teamModel = null;
        }
        teamModel = new TeamModel();
        currentLevel = levelInfo;
        teamModel.InitializeTeamList(currentLevel, mechDisplayManager);
        teamChooserUI.Initialize(teamModel, availableEntities);
        OnTeamChanged();
    }

    private void OnTeamChanged()
    {
        // MechStats currentMech = teamModel.TeamSpots[teamModel.CurrentSpotIndex].chosenMech;
        mechDisplayManager.DisplayMech(null);
        teamChooserUI.UpdateUI();
        // MechDisplayManager
    }

    public void StartLevel() {
        GameObject teamPersistorObject = new GameObject("TeamPersistorObject");
        TeamRosterPersistor roster = teamPersistorObject.AddComponent<TeamRosterPersistor>();


        
        Debug.Log("TeamSpots Count form controller: " + teamModel.TeamSpots.Count);
        roster.PrepTeamForLevel(currentLevel.levelStringName, teamModel.TeamSpots);
    }


    [System.Serializable]
    public class TeamSpot {
        public MechStats chosenMech;
        public CharacterStats chosenPilot;
    }




}