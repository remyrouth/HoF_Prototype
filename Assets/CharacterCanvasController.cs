using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCanvasController : MonoBehaviour
{
    [Header("Display Variables")]
    public PlayerController currentCharacter;
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
    public GameObject AbilityButton1;
    public GameObject AbilityButton1Background;
    public Text AbilityText1;
    public GameObject AbilityButton2;
    public GameObject AbilityButton2Background;
    public Text AbilityText2;
    public GameObject AbilityButton3;
    public GameObject AbilityButton3Background;
    public Text AbilityText3;

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

        if (newCharacterScript.RetrieveMechInfo().AbilitySlot1.type != MechStats.AbilityType.None) {
            // abilityUIClass1
            AbilityButton1.SetActive(true);
            AbilityText1.text = mechInfo.AbilitySlot1.type.ToString();
        } else {
            AbilityButton1.SetActive(false);
        }
        if (newCharacterScript.RetrieveMechInfo().AbilitySlot2.type != MechStats.AbilityType.None) {
            AbilityButton2.SetActive(true);
            AbilityText2.text = mechInfo.AbilitySlot2.type.ToString();
        } else {
            AbilityButton2.SetActive(false);
        }
        if (newCharacterScript.RetrieveMechInfo().AbilitySlot3.type != MechStats.AbilityType.None) {
            AbilityButton3.SetActive(true);
            AbilityText3.text = mechInfo.AbilitySlot3.type.ToString();
        } else {
            AbilityButton3.SetActive(true);
        }
    }

    [System.Serializable]
    public class AbilityButtonClass {
        public GameObject parentObject;
        public GameObject buttonBackground;
        public Text abilityTitleText;
        public Text descriptionText;
        public Text clarityCostText;

        public void ChangeActiveStatus(bool newStatus) {
            parentObject.SetActive(newStatus);
        }

        public void DisplayAbilityInfo() {

        }

        public void ChooseSpecificAbility() {
            // parentObject.SetActive(newStatus);
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



        currentCharacter = null;
        pilotPortait.sprite = null;
        pilotPortait.gameObject.SetActive(false);

        actionMenu.SetActive(false);
        abilityMenu.SetActive(false);
    }
}
