using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeamRosterPersistor : MonoBehaviour
{
    [SerializeField] private List<TeamChooserController.TeamSpot> teamSpots = new List<TeamChooserController.TeamSpot>();
    [SerializeField] private GameObject unitPlacementControllerPrefab;
    private CombatStateController combatStateController;
    private static TeamRosterPersistor instance;

    public void PrepTeamForLevel(string sceneName, List<TeamChooserController.TeamSpot> newTeamRoster, GameObject placementPrefab) {
        unitPlacementControllerPrefab = placementPrefab;
        // Debug.Log("Prepped");
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

    private void PlaceRosterUsingExistingPieces() {
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


    // called by unity itself when a scene is loaded, but is subscribed to the 
    // unity manager itself using SceneManager.sceneLoaded
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // PlaceRosterUsingExistingPieces();
        PlaceTeamOnBoard();
    }


    private void PlaceTeamOnBoard() {
        PlayerController[] playerEntityList = FindObjectsOfType<PlayerController>();
        // List<GameObject> viablePlayerGameObjectList = FindObjectsOfType<GameObject>();

        foreach(PlayerController playerPiece in playerEntityList) {
            GameObject playerObject = playerPiece.gameObject;
            AIPlayerController AI = playerObject.GetComponent<AIPlayerController>();
            if (AI == null) {
                // this means we found a player object
                // viablePlayerGameObjectList.Add(playerObject);
                Destroy(playerObject);
            }
        }

        Debug.Log("Executed PlaceTeamOnBoard method");
        GameObject unitPlacer = Instantiate(unitPlacementControllerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        UnitPlacementController unitPlacementController = unitPlacer.GetComponent<UnitPlacementController>();
        unitPlacementController.InitializeFromTeamRosterPersistor(teamSpots);

    }

    private void OnDestroy()
    {
        // Unsubscribe from the event when the object is destroyed
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
