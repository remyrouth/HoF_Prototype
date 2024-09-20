using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class VNController : MonoBehaviour
{
    [SerializeField] private bool playsOnStart = false;
    [SerializeField] private List<DialogueSpeaker> leftSideSpeakers = new List<DialogueSpeaker>();
    [SerializeField] private List<DialogueSpeaker> rightSideSpeakers = new List<DialogueSpeaker>();


    [SerializeField] private List<DialogueSpoken> spokenDialogue = new List<DialogueSpoken>();

    [SerializeField] private int currentDialogueIndex = 0;


    [SerializeField] private Text dialogueTextObject;


    private void Start() {
        if (playsOnStart) {
            StartFirstDialogue();
        }
    }

    private void StartFirstDialogue() {
        if (spokenDialogue.Count > 0) {
            dialogueTextObject.text = spokenDialogue[currentDialogueIndex].textSpoken;
        }
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            if (currentDialogueIndex < spokenDialogue.Count - 1) {
                currentDialogueIndex++;
                dialogueTextObject.text = spokenDialogue[currentDialogueIndex].textSpoken;
            } else {
                Destroy(gameObject);
            }
        }
    }

    private IEnumerator FadeInImage(Image image, Color colorFade) {
        yield return null;
    }

    [System.Serializable]
    public class DialogueSpeaker {
        public bool spriteFaceLeftDefault = true;
        public Sprite characterSprite = null;
        public string characterName;
        public Vector2 defaultOffset = new Vector2(0f, 0f);
    }

    [System.Serializable]
    public class DialogueSpoken {
        public string textSpoken; 
    }
}
