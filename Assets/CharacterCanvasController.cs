using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCanvasController : MonoBehaviour
{
    [Header("Current Character")]
    public PlayerController currentCharacter;

    [Header("Display Variables")]
    public Image pilotPortait;
    public Image mechPortrait;
    public Image mechAbilityPortrait;
    public GameObject actionMenu;
    public GameObject abilityMenu;


    [Header("Main Action Button Variables")]
    public GameObject moveButtonMain;
    public GameObject moveButtonBackground;
    public GameObject attackButtonMain;
    public GameObject attackButtonBackground;
    public GameObject abilityButtonMain;
    public GameObject abilityButtonBackground;

    [Header("Ability Button Variables")]
    public AbilityButtonClass abilityUIClass1;
    public AbilityButtonClass abilityUIClass2;
    public AbilityButtonClass abilityUIClass3;

    private SelectionManager sm;

    private void Start() {
        sm = FindObjectOfType<SelectionManager>();

        actionMenu.SetActive(false);
        abilityMenu.SetActive(false);
    }


    // UI Methods
    public void DisplayCharacter(PlayerController newCharacterScript){
        currentCharacter = newCharacterScript;


        DetermineActionAvailability(newCharacterScript);

        moveButtonBackground.SetActive(false);
        attackButtonBackground.SetActive(false);

        // for action menu
        pilotPortait.gameObject.SetActive(true);
        mechPortrait.gameObject.SetActive(true);

        actionMenu.SetActive(true);
        abilityMenu.SetActive(false);


        // setting portraits
        pilotPortait.sprite = currentCharacter.RetrievePilotInfo().characterSprite;
        mechPortrait.sprite = currentCharacter.RetrieveMechInfo().characterSprite;

    }

    public void OpenAbilityMenu() {
        // we make sure its in viewing state in case they've already clicked an action button
        sm.ChangeToViewingState();
        actionMenu.SetActive(false);
        abilityMenu.SetActive(true);
        mechAbilityPortrait.gameObject.SetActive(true);

        DetermineAbilityAvailability(currentCharacter);


        mechAbilityPortrait.sprite = mechPortrait.sprite;
    }

    public void GoBackToActionMenu() {
        // we make sure its in viewing state in case they've already clicked an ability button
        sm.ChangeToViewingState();
        DisplayCharacter(currentCharacter);
    }

    // this was triggered by UI buttons
    public void SelectAbility(int abilitySlotNumber) {
        if (abilitySlotNumber == 1) {
            abilityUIClass1.SelectButton();
            abilityUIClass2.CleanupButton();
            abilityUIClass3.CleanupButton();
            sm.ChangeToAbilityState(abilityUIClass1.getCurretAbilityOfButton());
        } else if (abilitySlotNumber == 2) {
            abilityUIClass1.CleanupButton();
            abilityUIClass2.SelectButton();
            abilityUIClass3.CleanupButton();
            sm.ChangeToAbilityState(abilityUIClass2.getCurretAbilityOfButton());
        } else if (abilitySlotNumber == 3) {
            abilityUIClass1.CleanupButton();
            abilityUIClass2.CleanupButton();
            abilityUIClass3.SelectButton();
            sm.ChangeToAbilityState(abilityUIClass3.getCurretAbilityOfButton());
        } 
    }

    // Determines what buttons are available based on : 
    // if the entity is player controlled
    // if they've moved, attack, etc 
    // (basically if actions have not been used up)
    private void DetermineActionAvailability(PlayerController newCharacterScript) {
        if (newCharacterScript.isPlayerEntity) {
            
            if (newCharacterScript.hasAttackedYet) {
                attackButtonMain.SetActive(false);
            } else {
                attackButtonMain.SetActive(true);
            }

            if (newCharacterScript.hasMovedYet) {
                moveButtonMain.SetActive(false);
            } else {
                moveButtonMain.SetActive(true);
            }        

            if (newCharacterScript.hasUsedAbilityYet) {
                abilityButtonMain.SetActive(false);
            } else {
                abilityButtonMain.SetActive(true);
            }     

        } else {
            attackButtonMain.SetActive(false);
            moveButtonMain.SetActive(false);
            abilityButtonMain.SetActive(false);
        }
    }

    private void DetermineAbilityAvailability(PlayerController newCharacterScript) {
        MechStats mechInfo = newCharacterScript.RetrieveMechInfo();
        MechStats.AbilityMechSlot slot1 = mechInfo.AbilitySlot1;
        MechStats.AbilityMechSlot slot2 = mechInfo.AbilitySlot2;
        MechStats.AbilityMechSlot slot3 = mechInfo.AbilitySlot3;

        if (slot1.IsNotNoneType()) {
            abilityUIClass1.DisplayAbilityInfo(slot1);
        } else {
            abilityUIClass1.DisableAbilityButton();
        }
        if (slot2.IsNotNoneType()) {
            abilityUIClass2.DisplayAbilityInfo(slot2);
        } else {
            abilityUIClass2.DisableAbilityButton();
        }
        if (slot3.IsNotNoneType()) {
            abilityUIClass3.DisplayAbilityInfo(slot3);
        } else {
            abilityUIClass3.DisableAbilityButton();
        }
    }

    [System.Serializable]
    public class AbilityButtonClass {
        public GameObject parentObject;
        public GameObject buttonBackground;
        public Text abilityTitleText;
        public Text descriptionText;
        public Text clarityCostText;
        public Text rangeText;
        private MechStats.AbilityMechSlot currentAbility;

        public void DisableAbilityButton() {
            parentObject.SetActive(false);
        }

        public void DisplayAbilityInfo(MechStats.AbilityMechSlot slot) {
            currentAbility = slot;
            parentObject.SetActive(true);
            abilityTitleText.text = slot.GetAbilityType().ToString();
            descriptionText.text = slot.GetAbilityTypeDescription();
            clarityCostText.text = "Clarity Cost: " + slot.GetClarityCost();
            rangeText.text = "Range: " + slot.GetMinimumRange().ToString() + "/" + slot.GetMaximumRange().ToString();
            if (slot.GetMaximumRange() == 0) {
                rangeText.text = "Range: Self targeting";
            }
        }
        
        // triggered and called by canvas controller which was triggered by UI Button
        public void SelectButton() {
            buttonBackground.SetActive(true);
        }

        public void CleanupButton() {
            buttonBackground.SetActive(false);
        }

        public MechStats.AbilityMechSlot getCurretAbilityOfButton() {
            return currentAbility;
        }
    }

    // Turn order interaction methods
    public void BeginMoveSystem () {
        // Debug.Log("Button Triggered Move Method");
        sm.ChangeToMovingState();

        moveButtonBackground.SetActive(true);
        attackButtonBackground.SetActive(false);
        abilityButtonBackground.SetActive(false);
    }

    public void BeginAttackSystem () {
        // Debug.Log("Button Triggered Attack Method");
        sm.ChangeToAttackingState();

        moveButtonBackground.SetActive(false);
        attackButtonBackground.SetActive(true);
        abilityButtonBackground.SetActive(false);
    }

    // Clean up Method
    public void MenuCleanup() {
        moveButtonBackground.SetActive(false);
        attackButtonBackground.SetActive(false);


        // Main Action Menu
        currentCharacter = null;
        pilotPortait.sprite = null;
        pilotPortait.gameObject.SetActive(false);

        // Ability Menu
        abilityUIClass1.CleanupButton();
        abilityUIClass2.CleanupButton();
        abilityUIClass3.CleanupButton();

        actionMenu.SetActive(false);
        abilityMenu.SetActive(false);
    }
}
