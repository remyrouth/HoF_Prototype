using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCanvasController : MonoBehaviour
{
    [Header("Current Character")]
    public PlayerController currentCharacter;
    private SelectionManager sm;

    [Header("Display Variables")]
    public Image pilotPortait;
    public Image pilotAttackPortait;
    public Image mechPortrait;
    public Image mechAbilityPortrait;
    public GameObject actionMenu;
    public GameObject abilityMenu;
    public GameObject attackMenu;

    [Header("Main Action Texts")]

    public Text mechHealthText;
    public Text pilotHealthText;
    public Text moveSpeedText;

    [Header("Main Action Button Variables")]
    public AbilityButtonClass moveUIClass;
    public AbilityButtonClass attackUIClass;
    public AbilityButtonClass abilityUIClass;

    [Header("Main Attack Button Variables")]
    public AbilityButtonClass laserAttackUIClass;
    public AbilityButtonClass ballisticAttackUIClass;
    public AbilityButtonClass comboAttackUIClass;

    [Header("Ability Button Variables")]
    public AbilityButtonClass abilityUIClass1;
    public AbilityButtonClass abilityUIClass2;
    public AbilityButtonClass abilityUIClass3;

    private void Start() {
        sm = FindObjectOfType<SelectionManager>();

        actionMenu.SetActive(false);
        abilityMenu.SetActive(false);
        attackMenu.SetActive(false);


        // for action menu
        pilotPortait.gameObject.SetActive(true);
        mechPortrait.gameObject.SetActive(true);
        pilotAttackPortait.gameObject.SetActive(true);
        mechAbilityPortrait.gameObject.SetActive(true);
    }


    // UI Methods
    public void DisplayCharacter(PlayerController newCharacterScript){
        if (newCharacterScript == null) {
            return;
        }
        currentCharacter = newCharacterScript;

        DetermineActionAvailability(newCharacterScript);
        ShowCharacterText(newCharacterScript);

        moveUIClass.CleanupButton();
        attackUIClass.CleanupButton();
        abilityUIClass.CleanupButton();

        actionMenu.SetActive(true);
        abilityMenu.SetActive(false);
        attackMenu.SetActive(false);


        // setting portraits
        pilotPortait.sprite = currentCharacter.RetrievePilotInfo().GetCharacterSprite();
        mechPortrait.sprite = currentCharacter.RetrieveMechInfo().characterSprite;
        pilotAttackPortait.sprite = currentCharacter.RetrievePilotInfo().GetCharacterSprite();
        mechAbilityPortrait.sprite = currentCharacter.RetrieveMechInfo().characterSprite;

    }

    private void ShowCharacterText(PlayerController newCharacterScript) {
        CharacterStats pilot = newCharacterScript.RetrievePilotInfo();
        MechStats mech = newCharacterScript.RetrieveMechInfo();

        mechHealthText.text = "Mech Health: " + newCharacterScript.GetMechHealth() + "/" + newCharacterScript.GetMechMaxHealth().ToString();
        pilotHealthText.text = "Pilot Health: " + newCharacterScript.GetPilotHealth() + "/" + pilot.GetPilotHealth().ToString();
        moveSpeedText.text = "Move Speed: " +pilot.GetPilotSpeed().ToString();

    }

    public void OpenAbilityMenu() {
        // we make sure its in viewing state in case they've already clicked an action button
        sm.ChangeToViewingState();
        actionMenu.SetActive(false);
        abilityMenu.SetActive(true);
        attackMenu.SetActive(false);
        mechAbilityPortrait.gameObject.SetActive(true);

        DetermineAbilityAvailability(currentCharacter);


        mechAbilityPortrait.sprite = mechPortrait.sprite;
    }

    public void GoBackToActionMenu() {
        sm.ChangeToViewingState();
        DisplayCharacter(currentCharacter);
    }

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

    public void SelectAttackOption(int attackOption) {
        if (attackOption == 1) {
            laserAttackUIClass.SelectButton();
            ballisticAttackUIClass.CleanupButton();
            comboAttackUIClass.CleanupButton();

            // int power = currentCharacter.RetrievePilotInfo().GetLaserStrength();
            // int range = currentCharacter.RetrievePilotInfo().GetLaserRange();
            // MechStats.AbilityMechSlot tempSlot = CreateAttackSlotOption(power, range);
            Debug.Log("Clicked Lazer");
            // sm.ChangeToAbilityState(tempSlot);
            MechStats.AbilityMechSlot tempSlot = laserAttackUIClass.getCurretAbilityOfButton();
            sm.ChangeToAbilityState(tempSlot);
        } else if (attackOption == 2) {
            laserAttackUIClass.CleanupButton();
            ballisticAttackUIClass.SelectButton();
            comboAttackUIClass.CleanupButton();

            // int power = currentCharacter.RetrievePilotInfo().GetBallisticStrength();
            // int range = currentCharacter.RetrievePilotInfo().GetBallisticRange();
            // MechStats.AbilityMechSlot tempSlot = CreateAttackSlotOption(power, range);
            Debug.Log("Used Ballistic");
            // sm.ChangeToAbilityState(tempSlot);
            MechStats.AbilityMechSlot tempSlot = ballisticAttackUIClass.getCurretAbilityOfButton();
            sm.ChangeToAbilityState(tempSlot);
        } else if (attackOption == 3) {
            laserAttackUIClass.CleanupButton();
            ballisticAttackUIClass.CleanupButton();
            comboAttackUIClass.SelectButton();


            // int power = currentCharacter.RetrievePilotInfo().GetLaserStrength();
            // int range = currentCharacter.RetrievePilotInfo().GetLaserRange();
            // MechStats.AbilityMechSlot tempSlot = CreateAttackSlotOption(power, range);
            Debug.Log("Used Combo");
            MechStats.AbilityMechSlot tempSlot = comboAttackUIClass.getCurretAbilityOfButton();
            sm.ChangeToAbilityState(tempSlot);
            // sm.ChangeToAbilityState(tempSlot);
        } 
    }
    public MechStats.AbilityMechSlot CreateAttackSlotOption(int power, int attackRange) {
        // why are these 0? because standard attack options do not cost 
        // clarity to use/cast. And the minimum range of these abilities is
        // always zero. Other abilities may have a minimum range, 
        // standard attack options do not
        MechStats.AbilityMechSlot tempSlot = new MechStats.AbilityMechSlot();
        int clarityCost = 0;
        int minimumRange = 0;
        tempSlot.SetValues(power, clarityCost, minimumRange, attackRange);
        tempSlot.SetAbilityType(MechStats.AbilityType.Laser);

        return tempSlot;
    }

    private void DetermineActionAvailability(PlayerController newCharacterScript) {
        if (newCharacterScript.isPlayerEntity) {
            
            if (newCharacterScript.hasAttackedYet) {
                attackUIClass.DisableAbilityButton();
                abilityUIClass.DisableAbilityButton();
            } else {
                attackUIClass.enableButton();
                abilityUIClass.enableButton();
            }

            if (newCharacterScript.hasMovedYet) {
                moveUIClass.DisableAbilityButton();
            } else {
                moveUIClass.enableButton();
            }        
    

        } else {
            attackUIClass.DisableAbilityButton();
            abilityUIClass.DisableAbilityButton();
            moveUIClass.DisableAbilityButton();
        }
    }

    private void DetermineAbilityAvailability(PlayerController newCharacterScript) {
        MechStats mechInfo = newCharacterScript.RetrieveMechInfo();
        MechStats.AbilityMechSlot slot1 = mechInfo.AbilitySlot1;
        MechStats.AbilityMechSlot slot2 = mechInfo.AbilitySlot2;
        MechStats.AbilityMechSlot slot3 = mechInfo.AbilitySlot3;

        if (slot1.IsNotNoneType()) {
            abilityUIClass1.DisplayAbilityInfo(slot1);
            abilityUIClass1.CleanupButton();
        } else {
            abilityUIClass1.DisableAbilityButton();
        }
        if (slot2.IsNotNoneType()) {
            abilityUIClass2.DisplayAbilityInfo(slot2);
            abilityUIClass2.CleanupButton();
        } else {
            abilityUIClass2.DisableAbilityButton();
        }
        if (slot3.IsNotNoneType()) {
            abilityUIClass3.DisplayAbilityInfo(slot3);
            abilityUIClass3.CleanupButton();
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
        public Text damageText;
        public Text rangeText;
        private MechStats.AbilityMechSlot currentAbility;

        public void DisableAbilityButton() {
            parentObject.SetActive(false);
        }

        public void enableButton() {
            parentObject.SetActive(true);
            SelectButton();
        }

        public void DisplayAbilityInfo(MechStats.AbilityMechSlot slot) {
            currentAbility = slot;
            enableButton();
            abilityTitleText.text = slot.GetAbilityType().ToString();
            descriptionText.text = slot.GetAbilityTypeDescription();
            clarityCostText.text = "Clarity Cost: " + slot.GetClarityCost();
            rangeText.text = "Range: " + slot.GetMinimumRange().ToString() + "/" + slot.GetMaximumRange().ToString();
            if (slot.GetMaximumRange() == 0) {
                rangeText.text = "Range: Self targeting";
            }
        }

        public void DisplayAttackInfo(MechStats.AbilityMechSlot slot) {
            currentAbility = slot;
            rangeText.text = "RNG: " + slot.GetMaximumRange();
            damageText.text = "DMG: " + slot.GetIntPower();


            CleanupButton();
            enableButton();
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

        moveUIClass.enableButton();
        attackUIClass.CleanupButton();
        abilityUIClass.CleanupButton();
    }

    public void BeginAttackSystem () {
        // Debug.Log("Button Triggered Attack Method");
        // sm.ChangeToAttackingState();
        sm.ChangeToViewingState();
        actionMenu.SetActive(false);
        abilityMenu.SetActive(false);
        attackMenu.SetActive(true);

        int laserPower = currentCharacter.RetrievePilotInfo().GetLaserStrength();
        int laserRange = currentCharacter.RetrievePilotInfo().GetLaserRange();
        MechStats.AbilityMechSlot tempLaserSlot = CreateAttackSlotOption(laserPower, laserRange);
        laserAttackUIClass.DisplayAttackInfo(tempLaserSlot);

        int ballisticPower = currentCharacter.RetrievePilotInfo().GetBallisticStrength();
        int ballisticRange = currentCharacter.RetrievePilotInfo().GetBallisticRange();
        MechStats.AbilityMechSlot tempBallisticSlot = CreateAttackSlotOption(ballisticPower, ballisticRange);
        ballisticAttackUIClass.DisplayAttackInfo(tempBallisticSlot);

        int comboPower = currentCharacter.RetrievePilotInfo().GetLaserStrength();
        int comboRange = currentCharacter.RetrievePilotInfo().GetLaserRange();
        MechStats.AbilityMechSlot tempComboSlot = CreateAttackSlotOption(comboPower, comboRange);
        comboAttackUIClass.DisplayAttackInfo(tempComboSlot);


        laserAttackUIClass.CleanupButton();
        ballisticAttackUIClass.CleanupButton();
        comboAttackUIClass.CleanupButton();
    }

    // Clean up Method
    public void MenuCleanup() {
        moveUIClass.DisableAbilityButton();
        attackUIClass.DisableAbilityButton();


        // Main Action Menu
        currentCharacter = null;
        pilotPortait.sprite = null;
        moveUIClass.CleanupButton();

        // Ability Menu
        abilityUIClass1.CleanupButton();
        abilityUIClass2.CleanupButton();
        abilityUIClass3.CleanupButton();

        laserAttackUIClass.CleanupButton();
        ballisticAttackUIClass.CleanupButton();
        comboAttackUIClass.CleanupButton();

        actionMenu.SetActive(false);
        abilityMenu.SetActive(false);
        attackMenu.SetActive(false);
    }
}
