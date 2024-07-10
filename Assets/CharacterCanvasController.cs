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
    private Canvas canvasComponent;


    [Header("Button Variables")]
    public GameObject moveButtonMain;
    public GameObject moveButtonBackground;
    public GameObject attackButtonMain;
    public GameObject attackButtonBackground;

    private SelectionManager sm;

    private void Start() {
        sm = FindObjectOfType<SelectionManager>();
        canvasComponent = GetComponent<Canvas>();
        canvasComponent.enabled = false;
    }

    public void DisplayCharacter(PlayerController newCharacterScript){
        currentCharacter = newCharacterScript;


        DetermineButtonAvailability(newCharacterScript);

        moveButtonBackground.SetActive(false);
        attackButtonBackground.SetActive(false);


        pilotPortait.gameObject.SetActive(true);
        mechPortrait.gameObject.SetActive(true);
        canvasComponent.enabled = true;
        pilotPortait.sprite = currentCharacter.RetrievePilotInfo().characterSprite;
        mechPortrait.sprite = currentCharacter.RetrieveMechInfo().characterSprite;

    }

    // Determines what buttons are available based on : 
    // if the entity is player controlled
    // if they've moved, attack, etc 
    // (basically if actions have not been used up)
    private void DetermineButtonAvailability(PlayerController newCharacterScript) {
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

        } else {
            attackButtonMain.SetActive(false);
            moveButtonMain.SetActive(false);
        }
    }


    // Turn order interaction methods
    public void BeginMoveSystem () {
        // Debug.Log("Button Triggered Move Method");
        sm.ChangeToMovingState();

        moveButtonBackground.SetActive(true);
        attackButtonBackground.SetActive(false);
    }

    public void BeginAttackSystem () {
        // Debug.Log("Button Triggered Attack Method");
        sm.ChangeToAttackingState();

        moveButtonBackground.SetActive(false);
        attackButtonBackground.SetActive(true);
    }

    // Clean up Method
    public void MenuCleanup() {
        moveButtonBackground.SetActive(false);
        attackButtonBackground.SetActive(false);



        currentCharacter = null;
        pilotPortait.sprite = null;
        pilotPortait.gameObject.SetActive(false);
        canvasComponent.enabled = false;
    }
}
