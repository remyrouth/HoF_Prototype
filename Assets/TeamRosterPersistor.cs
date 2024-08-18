using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeamRosterPersistor : MonoBehaviour
{
    public List<TeamChooserController.TeamSpot> teamSpots = new List<TeamChooserController.TeamSpot>();
    
    private static TeamRosterPersistor instance;

    public void PrepTeamForLevel(string sceneName, List<TeamChooserController.TeamSpot> newTeamRoster) {
        Debug.Log("Prepped");
        teamSpots = newTeamRoster;
        SceneManager.LoadScene(sceneName);
    }

    private void Awake()
    {
        // Singleton pattern to ensure only one instance
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to the scene loaded event
        }
        else
        {
            Destroy(gameObject); // Destroy duplicates
        }
    }

    private void OnEnable()
    {
        PlaceRoster();
    }

    private void PlaceRoster() {
        // Find replaceable player objects
        PlayerController[] players = FindObjectsOfType<PlayerController>();
        List<PlayerController> playersToReplace = new List<PlayerController>();
        foreach (PlayerController player in players) {
            if (player.canReplacePlayerEntity()) {
                playersToReplace.Add(player);
            }
        }

        List<TeamChooserController.TeamSpot> validTeamSpot = new List<TeamChooserController.TeamSpot>();
        foreach (TeamChooserController.TeamSpot spot in teamSpots) {
            if (spot.chosenMech && spot.chosenPilot) {
                validTeamSpot.Add(spot);
            }
        }

        Debug.Log("playersToReplace: " + playersToReplace.Count);
        Debug.Log("validTeamSpot: " + validTeamSpot.Count);

        if (validTeamSpot.Count == playersToReplace.Count) {
            for (int i = 0; i < playersToReplace.Count; i++) {
                playersToReplace[i].SetPilotAndMechFromRosterScript(validTeamSpot[i].chosenPilot, validTeamSpot[i].chosenMech);
            }
        }

        if (validTeamSpot.Count > playersToReplace.Count) {
            for (int i = 0; i < playersToReplace.Count; i++) {
                playersToReplace[i].SetPilotAndMechFromRosterScript(validTeamSpot[i].chosenPilot, validTeamSpot[i].chosenMech);
            }
            // Excess valid spots are ignored
        }

        if (validTeamSpot.Count < playersToReplace.Count) {
            for (int i = 0; i < validTeamSpot.Count; i++) {
                playersToReplace[i].SetPilotAndMechFromRosterScript(validTeamSpot[i].chosenPilot, validTeamSpot[i].chosenMech);
            }
            // Delete excess players
            for (int i = validTeamSpot.Count; i < playersToReplace.Count; i++) {
                Destroy(playersToReplace[i].gameObject);
            }
        }
    }

    // Method to be called on scene change
    private void OnSceneChange()
    {
        Debug.Log("Scene changed! Executing method...");
        PlaceRoster();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        OnSceneChange(); // Call your method when a new scene is loaded
    }

    private void OnDestroy()
    {
        // Unsubscribe from the event when the object is destroyed
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
