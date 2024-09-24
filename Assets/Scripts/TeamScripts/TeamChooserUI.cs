using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamChooserUI : MonoBehaviour
{
    [Header("Slot Menu Variables")]
    [SerializeField] private GameObject SlotChoicePrefab; // a prefab with a script to show a pilot or mech to choose
    [SerializeField] private List<GameObject> SlotChoiceObjectList = new List<GameObject>(); // doesn't need to be serialized
    [SerializeField] private GameObject slotChoiceObjectsParent;
    [Header("Pilot Menu Variables")]
    [SerializeField] private List<GameObject> pilotChoiceObjectList = new List<GameObject>(); // doesn't need to be serialized
    [SerializeField] private GameObject pilotChoiceObjectsParent;
    [Header("Mech Menu Variables")]
    [SerializeField] private List<GameObject> mechChoiceObjectList = new List<GameObject>(); // doesn't need to be serialized
    [SerializeField] private GameObject mechChoiceObjectsParent;


    // current state variables
    private bool isPaused = false;
    [SerializeField] private currentUIState currentStateOfUI = currentUIState.choosingSlot;
    [SerializeField] private teamSlotClass currentSlot; // doesn't need to be serialized
    [SerializeField] private TeamModel currentTeam; // doesn't need to be serialized

    public enum currentUIState {
        choosingSlot,
        choosingPilotForSlot,
        choosingMechForSlot,
    }

    // a button from team option buttons has updated the team model, we must
    // get the current index we're at from the team model, and update the images
    // on the right hand side

    public void UpdatePortraits() {

    }

    public void Initialize(TeamModel model, TeamBuilder newEntityList)
    {
        // totalAvailableEntities = newEntityList;
        // teamModel = model;
        // pilotArrayHolder.CreateEntityPortaits(totalAvailableEntities, spriteOptionHolderPrefab, teamModel);
        // mechArrayHolder.CreateEntityPortaits(totalAvailableEntities, spriteOptionHolderPrefab, teamModel);
        MakeSlotsUI(model.TeamSpots.Count);
    }

    private void MakeSlotsUI(int maxSlots) {
        for(int i = 0; i < maxSlots; i++) {
            GameObject slotUIObject = Instantiate(SlotChoicePrefab, slotChoiceObjectsParent);
            SlotChoice slotChoice = slotUIObject.GeComponent<SlotChoice>();
            int SlotNum = i;
            SlotChoice.InitializeFromTeamChooserUI(SlotNum, currentTeam);
        }
    }

    // called by game state manager
    public void SetPauseState(bool newPauseState) {
        isPaused = newPauseState;
    }

    [System.Serializable]
    public class teamSlotClass {
        MechStats mech;
        CharacterStats pilot;
    }


}
