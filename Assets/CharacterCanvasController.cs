using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCanvasController : MonoBehaviour
{
    public PlayerController currentCharacter;


    public Image characterPortrait;
    private Canvas canvasComponent;

    private void Start() {
        canvasComponent = GetComponent<Canvas>();
        canvasComponent.enabled = false;
    }



    public void DisplayCharacter(PlayerController newCharacterScript){
        currentCharacter = newCharacterScript;

        characterPortrait.gameObject.SetActive(true);
        canvasComponent.enabled = true;
        characterPortrait.sprite = currentCharacter.characterInfo.characterSprite;

    }

    public void MenuCleanup() {
        currentCharacter = null;
        characterPortrait.sprite = null;
        characterPortrait.gameObject.SetActive(false);
        canvasComponent.enabled = false;
    }
}
