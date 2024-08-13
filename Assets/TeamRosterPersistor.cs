using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



// this is a script which is created by the team chooser controller scripting group.
// it essentially takes in a team, and takes it to the intended level scene
public class TeamRosterPersistor : MonoBehaviour
{
    List<TeamChooserController.TeamSpot> teamSpots = new List<TeamChooserController.TeamSpot>();
    
    public void PrepTeamForLevel(string sceneName, List<TeamChooserController.TeamSpot> newTeamRoster) {
        teamSpots = newTeamRoster;
        SceneManager.LoadScene(sceneName);
    }
    private void Awake()
    {
        // Prevent this GameObject (and its components) from being destroyed on scene load.
        DontDestroyOnLoad(this.gameObject);

        if (teamSpots.Count > 0) {
            PlaceRoster();
            Destroy(gameObject);
        }
    }

    private void PlaceRoster() {
        // find replaceable player objects
        PlayerController[] players = FindObjectsOfType<PlayerController>();
        List<PlayerController> playersToReplace = new List<PlayerController>();
        foreach (PlayerController player in players) {
            if (player.canReplacePlayerEntity()) {
                playersToReplace.Add(player);
            }
        }
        List<TeamChooserController.TeamSpot> validSpot = new List<TeamChooserController.TeamSpot>();

        foreach (TeamChooserController.TeamSpot spot in teamSpots) {
            if (spot.chosenMech && spot.chosenPilot) {
                validSpot.Add(spot);
            }
        }

        // Pair valid spots with players to replace
        int count = Mathf.Min(validSpot.Count, playersToReplace.Count);

        for (int i = 0; i < count; i++) {
            playersToReplace[i].SetPilotAndMechFromRosterScript(validSpot[i].chosenPilot, validSpot[i].chosenMech);
        }

        // Delete excess players
        for (int i = count; i < playersToReplace.Count; i++) {
            Destroy(playersToReplace[i].gameObject);
        }

    }
}
