using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCanvasController : MonoBehaviour
{
    public PlayerController currentCharacter;
    public Image characterPortrait;
    private Canvas canvasComponent;

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

        moveButtonBackground.SetActive(false);
        attackButtonBackground.SetActive(false);


        characterPortrait.gameObject.SetActive(true);
        canvasComponent.enabled = true;
        characterPortrait.sprite = currentCharacter.RetrievePilotInfo().characterSprite;

    }

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

    public void MenuCleanup() {
        moveButtonBackground.SetActive(false);
        attackButtonBackground.SetActive(false);



        currentCharacter = null;
        characterPortrait.sprite = null;
        characterPortrait.gameObject.SetActive(false);
        canvasComponent.enabled = false;
    }
}
