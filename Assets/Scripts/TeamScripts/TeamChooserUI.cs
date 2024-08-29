using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamChooserUI : MonoBehaviour
{
    [SerializeField] private Text teamSizeText;
    [SerializeField] private Text levelDescriptionText;


    [SerializeField] private OptionsArrayHolder pilotArrayHolder;
    [SerializeField] private OptionsArrayHolder mechArrayHolder;
    [SerializeField] private GameObject spriteOptionHolderPrefab;


    private TeamModel teamModel;
    private TeamBuilder totalAvailableEntities;

    public void Initialize(TeamModel model, TeamBuilder newEntityList)
    {
        totalAvailableEntities = newEntityList;
        teamModel = model;
        pilotArrayHolder.CreateEntityPortaits(totalAvailableEntities, spriteOptionHolderPrefab, teamModel);
        mechArrayHolder.CreateEntityPortaits(totalAvailableEntities, spriteOptionHolderPrefab, teamModel);
    }

    public void UpdatePortraits() {
        TeamChooserController.TeamSpot spot = teamModel.RetriveCurrentTeamSpot();
            // Finish this

        if (spot.chosenMech) {
            MechStats mech = spot.chosenMech;
            string primaryString = "Health: " + mech.GetMechHealth() + "\n" 
                + "Class: " + mech.GetMechType().ToString()+ "\n"
                + "Clarity Max: " + mech.GetMechMaxClarity().ToString();
            string abilities = "Ability1: " + mech.AbilitySlot1.GetAbilityType().ToString() + "\n" + 
            "Ability2: " + mech.AbilitySlot2.GetAbilityType().ToString() + "\n" + 
            "Ability3: " + mech.AbilitySlot3.GetAbilityType().ToString();
            string secondaryString = abilities;
            Sprite entitySprite = mech.GetMechSprite();

            mechArrayHolder.ChangeToPortraitMode(entitySprite, primaryString, secondaryString);
        } else {
            mechArrayHolder.CreateEntityPortaits(totalAvailableEntities, spriteOptionHolderPrefab, teamModel);
        }

        if (spot.chosenPilot) {
            CharacterStats pilot = spot.chosenPilot;
            string primaryString = "Health: " + pilot.GetPilotHealth() + "\n" 
                + "Speed: " + pilot.ToString()+ "\n"
                + "Clarity Gain: " + pilot.GetMoveClarity().ToString();
            string secondaryString = "";
            Sprite entitySprite = pilot.GetCharacterSprite();
            pilotArrayHolder.ChangeToPortraitMode(entitySprite, primaryString, secondaryString);
        } else {
            pilotArrayHolder.CreateEntityPortaits(totalAvailableEntities, spriteOptionHolderPrefab, teamModel);
        }

    }

    // button activated
    public void CancelPortrait(bool cancelsPilot) {
        if (cancelsPilot) {
            teamModel.UpdatePilot(null);
        } else {
            teamModel.UpdateMech(null);
        }

        UpdatePortraits();
    }

    public void UpdateUI() {
        UpdateTeamSpotText();
    }

    private void UpdateTeamSpotText() {
        int currentTeamSpotIndex = teamModel.CurrentSpotIndex;
        int teamMemberMax = teamModel.TeamSpots.Count;
        teamSizeText.text = "" + (currentTeamSpotIndex+1).ToString() + " / " + teamMemberMax.ToString();
    }

    // Methods to handle UI interactions
    public void OnTeamSpotChangeClicked(bool increase) {
        teamModel.ChangeCurrentSpot(increase);
        UpdateTeamSpotText();
        UpdatePortraits();
    }


    [System.Serializable]
    public class OptionsArrayHolder {
        [SerializeField] private Transform objectToHoldOptions;
        [SerializeField] private bool isForPilots = true;

        [SerializeField] private GameObject portraitGameObject;
        [SerializeField] private Image potraitImage;
        [SerializeField] private Text primaryText;
        [SerializeField] private Text secondaryText;


        public void CleanUpArrayHolder() {
            // Iterate through all children and destroy them
            for (int i = objectToHoldOptions.childCount - 1; i >= 0; i--) {
                // Object.Destroy(objectToHoldOptions.GetChild(i).gameObject);
                UnityEngine.Object.Destroy(objectToHoldOptions.GetChild(i).gameObject);
            }

            portraitGameObject.gameObject.SetActive(false);
            objectToHoldOptions.gameObject.SetActive(true);
        }

        public void ChangeToPortraitMode(Sprite portaitSprite, string primaryPortraitText, string secondaryPortraitText) {
            portraitGameObject.gameObject.SetActive(true);
            objectToHoldOptions.gameObject.SetActive(false);

            primaryText.text = primaryPortraitText;
            secondaryText.text = secondaryPortraitText;
            potraitImage.sprite = portaitSprite;
        }

        public void CreateEntityPortaits(TeamBuilder AvailableEntities, GameObject spriteOptionHolderPrefab, TeamModel teamModel) {
            CleanUpArrayHolder();
            int listLength = AvailableEntities.PilotLength();
            if (!isForPilots) {
                listLength = AvailableEntities.MechLength();
            }

            for (int i = 0; i < listLength; i++) {
                if (isForPilots) {
                    CharacterStats pilot = AvailableEntities.GetPilot(i);
                    if (!AlreadyHasPilotUsedOnTeam(pilot, teamModel)) {
                        GameObject instantiatedObject = Instantiate(spriteOptionHolderPrefab, objectToHoldOptions.position, objectToHoldOptions.rotation, objectToHoldOptions);
                        TeamSpotOptionController spotOptionObject = instantiatedObject.GetComponent<TeamSpotOptionController>();
                        spotOptionObject.BecomePilotOption(pilot, teamModel);
                    }
                } else {
                    MechStats mech = AvailableEntities.GetMech(i);
                    if (!AlreadyHasMechUsedOnTeam(mech, teamModel)) {
                        GameObject instantiatedObject = Instantiate(spriteOptionHolderPrefab, objectToHoldOptions.position, objectToHoldOptions.rotation, objectToHoldOptions);
                        TeamSpotOptionController spotOptionObject = instantiatedObject.GetComponent<TeamSpotOptionController>();
                        spotOptionObject.BecomeMechOption(mech, teamModel);
                    }
                }
            }
        }

        public bool AlreadyHasPilotUsedOnTeam(CharacterStats pilot, TeamModel teamModel) {
            foreach (TeamChooserController.TeamSpot spot in teamModel.TeamSpots) {
                if (spot.chosenPilot == pilot) {
                    return true;
                }
            }
            return false;

        }

        public bool AlreadyHasMechUsedOnTeam(MechStats mech,TeamModel teamModel) {
            foreach (TeamChooserController.TeamSpot spot in teamModel.TeamSpots) {
                if (spot.chosenMech == mech) {
                    return true;
                }
            }
            return false;
        }
    }

}
