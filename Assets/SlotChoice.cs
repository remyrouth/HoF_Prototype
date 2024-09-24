using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotChoice : MonoBehaviour
{
    public int SlotNumIndex = 0;
    private TeamModel teamModel;

    // called by TeamChooserUI
    public void InitializeFromTeamChooserUI(int newSlotNum, TeamModel newTeamModel) {
        SlotNumIndex = newSlotNum;
        teamModel = newTeamModel;
    }

    // called by button
    public void SelectSlot() {
        TeamChooserUI chooserUI = FindObjectOfType<TeamChooserUI>();
        if (chooserUI != null) {
            
        }
    }

    public void ClearSlotIndex() {
        TeamChooserUI chooserUI = FindObjectOfType<TeamChooserUI>();
        if (chooserUI != null) {
            
        }
    }
}
