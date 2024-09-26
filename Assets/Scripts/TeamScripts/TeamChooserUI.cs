using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamChooserUI : MonoBehaviour
{
    [Header("Slot Menu Variables")]
    [SerializeField] private GameObject SlotChoicePrefab; // a prefab with a script to show a pilot or mech to choose
    [SerializeField] private GameObject slotChoiceObjectsParent;
    [SerializeField] private Text slotText;

    [Header("General Menu Variables")]
    [SerializeField] private GameObject pilotMechFramePrefab;
    [SerializeField] private float maxSlotFramesPerPage = 4;
    [SerializeField] private float maxMechPilotFramesPerPage = 8;

    [Header("Pilot Menu Variables")]
    [SerializeField] private GameObject pilotChoiceObjectsParent;
    [SerializeField] private Image pilotImageDisplay;
    [Header("Mech Menu Variables")]
    [SerializeField] private GameObject mechChoiceObjectsParent;
    [SerializeField] private Image mechImageDisplay;

    [Header("Persistor Object")]
    [SerializeField] private GameObject persistorObject; // is not supposed to be a prefab
    // current state variables

    // [Header("Current State Variables")]
    private currentUIState currentStateOfUI = currentUIState.choosingSlot;
    private SlotChoice currentSlot; // doesn't need to be serialized
    private MapMarkerController.MapLevel currentLevelInfo; // doesn't need to be serialized
    private TeamBuilder availableEntities;
    private bool isPaused = false;
    // [SerializeField] private TeamModel currentTeam;


    public enum currentUIState {
        choosingSlot,
        choosingPilotForSlot,
        choosingMechForSlot,
    }
    public void Initialize(MapMarkerController.MapLevel newLevelInfo, TeamBuilder newEntityList)
    {
        availableEntities = newEntityList;
        currentLevelInfo = newLevelInfo;
        MakeSlotsUI();
        UpdateUI();
    }

    private void MakeSlotsUI() {
        foreach (Transform child in slotChoiceObjectsParent.transform)
        {
            // Destroy the children of the parent object to clean up
            Destroy(child.gameObject);
        }

        for(int i = 0; i < currentLevelInfo.teamMemberMax; i++) {
            GameObject slotUIObject = Instantiate(SlotChoicePrefab, slotChoiceObjectsParent.transform);
            SlotChoice slotChoice = slotUIObject.GetComponent<SlotChoice>();
            int SlotNum = i;
            slotChoice.InitializeFromTeamChooserUI(SlotNum);
        }
    }

    // 
    public void ReceiveSlot(SlotChoice newSlot) {
        currentSlot = newSlot;
        currentStateOfUI = currentUIState.choosingPilotForSlot;
        UpdateUI();
    }

    // this is called by the TeamSpotOptionController.cs script, which is a script that
    // holds a pilot or mech that the player will select in the team chooser
    public void UpdateCurrentSlot(CharacterStats pilot, MechStats mech) {
        if (currentSlot != null) {
            currentSlot.UpdateMechAndPilot(pilot, mech);
        }

        // if we're being sent a pilot but no mech it means we're choosing a pilot
        // and we should now be choosing mechs, the next part of choosing pairs
        // for team slots
        if (pilot != null  && mech == null) {
            currentStateOfUI = currentUIState.choosingMechForSlot;
            UpdateUI();
        }


        // if we're being sent a mech but no pilot it means we're choosing a mech
        // and should switch back to the slot tabs because we've chosen the last
        // component of a slot
        if (pilot == null  && mech != null) {
            currentStateOfUI = currentUIState.choosingSlot;
            UpdateUI();
        }
        
    }

    // must make amount of pilots, and also not use
    // pilots and mechs that were already used
    private void MakePilotOptionsUI() {
        foreach (Transform child in pilotChoiceObjectsParent.transform)
        {
            // Destroy the children of the parent object to clean up
            Destroy(child.gameObject);
        }

        foreach(CharacterStats pilot in availableEntities.pilots) {
            // basically if currentTeam.TeamSpots already has that
            // pilot in it, then do not create it in this UI
            
            if (!IsMechOrPilotAlreadyUsedInSlots(pilot, null)) {
                // making object
                GameObject pilotUIObject = Instantiate(pilotMechFramePrefab, pilotChoiceObjectsParent.transform);

                // initalizing script
                TeamSpotOptionController teamSpot_Pilot = pilotUIObject.GetComponent<TeamSpotOptionController>();
                teamSpot_Pilot.BecomePilotOption(pilot);
            }

        }
    }

    private void MakeMechOptionsUI() {
        foreach (Transform child in mechChoiceObjectsParent.transform)
        {
            // Destroy the children of the parent object to clean up
            Destroy(child.gameObject);
        }

        foreach(MechStats mech in availableEntities.mechs) {
            // basically if currentTeam.TeamSpots already has that
            // pilot in it, then do not create it in this UI
            if (!IsMechOrPilotAlreadyUsedInSlots(null, mech)) {
                // making object
                GameObject pilotUIObject = Instantiate(pilotMechFramePrefab, mechChoiceObjectsParent.transform);

                // initalizing script
                TeamSpotOptionController teamSpot_Pilot = pilotUIObject.GetComponent<TeamSpotOptionController>();
                teamSpot_Pilot.BecomeMechOption(mech);
            }

            
        }
    }

    private bool IsMechOrPilotAlreadyUsedInSlots(CharacterStats newPilot, MechStats newMech) {
        foreach (Transform slot in slotChoiceObjectsParent.transform)
        {
            SlotChoice slotScript = slot.gameObject.GetComponent<SlotChoice>();
            if (slotScript.ContainsMechOrPilot(newPilot, newMech)) {
                return true;
            }
        }
        return false;
    }

    // called by Button
    public void StartLevel() {
        // If this errors it means there is no UnitPlacemenCanvas
        // prefab in scene, fix that, just add it to the heirarchy
        persistorObject.SetActive(true);
        DontDestroyOnLoad(persistorObject);
        TeamRosterPersistor persistorScript = persistorObject.GetComponent<TeamRosterPersistor>();
        List<TeamChooserController.TeamSpot> newTeamRoster = new List<TeamChooserController.TeamSpot>();

        // assemble team here
        foreach (Transform slot in slotChoiceObjectsParent.transform)
        {
            SlotChoice slotScript = slot.gameObject.GetComponent<SlotChoice>();
            newTeamRoster.Add(slotScript.CreateTeamSpot());

        }

        persistorScript.PrepTeamForLevel(currentLevelInfo.levelStringName, newTeamRoster);
    }

    private void UpdateMechPilotFrameDisplays() {

        if (currentSlot == null) {
            pilotImageDisplay.gameObject.SetActive(false);
            mechImageDisplay.gameObject.SetActive(false);
        } else {
            TeamChooserController.TeamSpot pair = currentSlot.CreateTeamSpot();
            if (pair.chosenPilot != null) {
                pilotImageDisplay.gameObject.SetActive(true);
                pilotImageDisplay.sprite = pair.chosenPilot.GetCharacterSprite();
            } else {
                pilotImageDisplay.gameObject.SetActive(false);
            }

            if (pair.chosenMech != null) {
                mechImageDisplay.gameObject.SetActive(true);
                mechImageDisplay.sprite = pair.chosenMech.GetMechSprite();
            } else {
                mechImageDisplay.gameObject.SetActive(false);
            }
        }

    }


    private void UpdateUI() {
        UpdateMechPilotFrameDisplays();
        switch(currentStateOfUI) {
            case currentUIState.choosingSlot:
                // Debug.Log("ui is in choosing state");
                slotChoiceObjectsParent.SetActive(true);
                pilotChoiceObjectsParent.SetActive(false);
                mechChoiceObjectsParent.SetActive(false);
                slotText.text = "_ _ / " + currentLevelInfo.teamMemberMax;
                // slotText should show how many we have filled so far
                currentSlot = null;
                break;

            case currentUIState.choosingPilotForSlot:
                // Debug.Log("ui is in pilot state");
                slotChoiceObjectsParent.SetActive(false);
                pilotChoiceObjectsParent.SetActive(true);
                mechChoiceObjectsParent.SetActive(false);
                slotText.text = "" + currentSlot.GetSlotNumText() + "/ " + currentLevelInfo.teamMemberMax;
                MakePilotOptionsUI();
                break;

            case currentUIState.choosingMechForSlot:
                // Debug.Log("ui is in mech state");
                slotChoiceObjectsParent.SetActive(false);
                pilotChoiceObjectsParent.SetActive(false);
                mechChoiceObjectsParent.SetActive(true);
                MakeMechOptionsUI();
                break;

            default:
                Debug.LogWarning("Unknown state");
                break;
        }
    }

    // called by game state manager
    public void SetPauseState(bool newPauseState) {
        isPaused = newPauseState;
    }


}
