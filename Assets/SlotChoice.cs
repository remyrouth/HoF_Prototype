using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotChoice : MonoBehaviour
{
    [SerializeField] private int slotNumIndex = 0;
    [SerializeField] private Text slotNumText;
    private MechStats mech;
    private CharacterStats pilot;

    // called by TeamChooserUI
    public void InitializeFromTeamChooserUI(int newSlotNum) {
        slotNumIndex = newSlotNum;
        if (slotNumText != null) {
            slotNumText.text = "Slot Number: " + (slotNumIndex + 1);
        }
    }
    // called by TeamChooserUI
    public void UpdateMechAndPilot(CharacterStats newPilot, MechStats newMech) {
        if (newPilot == null && newMech == null) {
            // means we're using ClearCurrentSlot to clear it
            mech = newMech;
            pilot = newPilot;
        } else if (newPilot != null && newMech == null) {
            // means we're being sent specifically a pilot from
            // TeamSpotOptionController --> TeamChooserUI --> SlotChoice
            // which comes in the form of a !null and null
            pilot = newPilot;
        } else if (newPilot == null && newMech != null) {
            // means we're being sent specifically a mech from
            // TeamSpotOptionController --> TeamChooserUI --> SlotChoice
            // which comes in the form of a null and !null
            mech = newMech;
        }
    }

    public string GetSlotNumText() {
        return slotNumText.text;
    }

    // called by button
    public void SelectSlot() {
        TeamChooserUI chooserUI = FindObjectOfType<TeamChooserUI>();
        if (chooserUI != null) {
            chooserUI.ReceiveSlot(this);
        }
    }

    // called by button
    public void ClearCurrentSlot() {
        UpdateMechAndPilot(null, null);
    }

    // called by TeamChooserUI when assembling a team to send to the persistor
    public TeamChooserController.TeamSpot CreateTeamSpot() {
        TeamChooserController.TeamSpot teamSpot = new TeamChooserController.TeamSpot();
        teamSpot.chosenMech = mech;
        teamSpot.chosenPilot = pilot;
        return teamSpot;
    }

    public bool ContainsMechOrPilot(CharacterStats newPilot, MechStats newMech) {
        if (newPilot == pilot && newPilot != null) {
            return true;
        }

        if (newMech == mech && newMech != null) {
            return true;
        }

        return false;
    }
}
