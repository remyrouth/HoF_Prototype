using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamSpotOptionController : MonoBehaviour
{
    // this is a script that
    // holds a pilot or mech 
    // that the player will select 
    // in the team chooser
    [SerializeField] private MechStats chosenMech;
    [SerializeField] private CharacterStats chosenPilot;
    [SerializeField] private Image entityPortrait;
    private bool isPaused = false;

    

    // initialized by team chooser
    public void BecomeMechOption(MechStats mechOption) {
        chosenMech = mechOption;
        chosenPilot = null;
        entityPortrait.gameObject.SetActive(true);
        Debug.Log("BecomeMechOption method executed: " + mechOption.GetMechSprite() != null);
        entityPortrait.sprite = mechOption.GetMechSprite();
    }

    // initialized by team chooser
    public void BecomePilotOption(CharacterStats pilotOption) {
        chosenMech = null;
        chosenPilot = pilotOption;
        entityPortrait.gameObject.SetActive(true);
        entityPortrait.sprite = pilotOption.GetCharacterSprite();
    }


    public void SendEntity() {
        if (isPaused) {
            return;
        }
        
        TeamChooserController teamChooserscript = FindObjectOfType<TeamChooserController>();
        if (teamChooserscript == null) {
            Debug.LogError("TeamChooserController does not exist. It should though. Designer Fucked up");
        }

        if (chosenMech == null && chosenPilot == null) {
            Debug.LogError("SendEntity should not function before being initialized. Something is wrong");
        } else {
            TeamChooserUI teamUIScript = FindObjectOfType<TeamChooserUI>();
            teamUIScript.UpdateCurrentSlot(chosenPilot, chosenMech);
        }
    }

    // called by game state manager
    public void SetPauseState(bool newPauseState) {
        isPaused = newPauseState;
    }





}
