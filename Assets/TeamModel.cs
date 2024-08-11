using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamModel
{
    public List<TeamChooserController.TeamSpot> TeamSpots { get; private set; }
    public int CurrentSpotIndex { get; private set; }
    public MapMarkerController.MapLevel CurrentLevel { get; private set; }

    public void InitializeTeamList(MapMarkerController.MapLevel levelInfo) {
        CurrentLevel = levelInfo;
        TeamSpots = new List<TeamChooserController.TeamSpot>();
        for(int i = 0; i < levelInfo.teamMemberMax; i++) {
            TeamSpots.Add(new TeamChooserController.TeamSpot());
        }
    }

    // Methods to modify the team composition
    public void UpdateMech(MechStats newMech) {
        TeamSpots[CurrentSpotIndex].chosenMech = newMech;
    }
    public void UpdatePilot(CharacterStats newPilot) {
        TeamSpots[CurrentSpotIndex].chosenPilot = newPilot;
    }
    public void ChangeCurrentSpot(bool increase) {
        if (increase) {
            CurrentSpotIndex++;
            CurrentSpotIndex = Mathf.Min(CurrentSpotIndex, CurrentLevel.teamMemberMax - 1);
        } else {
            CurrentSpotIndex--;
            CurrentSpotIndex = Mathf.Max(CurrentSpotIndex, 0);
        }
    }
}
