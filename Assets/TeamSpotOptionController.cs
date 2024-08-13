using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamSpotOptionController : MonoBehaviour
{
    public TeamModel teamModel;
    public MechStats chosenMech;
    public CharacterStats chosenPilot;
    public Image entityPortrait;

    // initialized by team chooser
    public void BecomeMechOption(MechStats mechOption, TeamModel newTeamModel) {
        teamModel = newTeamModel;
        chosenMech = mechOption;
        chosenPilot = null;
        entityPortrait.gameObject.SetActive(true);
        entityPortrait.sprite = mechOption.GetMechSprite();
    }

    // initialized by team chooser
    public void BecomePilotOption(CharacterStats pilotOption, TeamModel newTeamModel) {
        teamModel = newTeamModel;
        chosenMech = null;
        chosenPilot = pilotOption;
        entityPortrait.gameObject.SetActive(true);
        entityPortrait.sprite = pilotOption.GetCharacterSprite();
    }


    public void SendEntity() {
        TeamChooserController teamChooserscript = FindObjectOfType<TeamChooserController>();
        if (teamChooserscript == null) {
            Debug.LogError("TeamChooserController does not exist. It should though");
        }



        if (chosenMech == null && chosenPilot == null) {
            Debug.LogError("SendEntity should not function before being initialized");
        } else if (chosenMech == null && chosenPilot != null) {
            // teamChooserscript.UpdatePilotCurrentSpotIndex(chosenPilot);
            teamModel.UpdatePilot(chosenPilot);
            TeamChooserUI teamUIScript = FindObjectOfType<TeamChooserUI>();
            teamUIScript.UpdatePortraits();
        } else if (chosenMech != null && chosenPilot == null) {
            // teamChooserscript.UpdateMechCurrentSpotIndex(chosenMech);
            teamModel.UpdateMech(chosenMech);
            TeamChooserUI teamUIScript = FindObjectOfType<TeamChooserUI>();
            teamUIScript.UpdatePortraits();
        }
    }





}
