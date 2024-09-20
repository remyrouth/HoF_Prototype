using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class VNController : MonoBehaviour
{
    [Header("Activates on Start, bool")]
    [SerializeField] private bool playsOnStart = false;
    [Header("List of speakers on left side")]
    [SerializeField] private List<DialogueSpeaker> leftSideSpeakers = new List<DialogueSpeaker>();
    [SerializeField] private Image leftSpeakerImage;
    [Header("List of speakers on right side")]
    [SerializeField] private List<DialogueSpeaker> rightSideSpeakers = new List<DialogueSpeaker>();
    [SerializeField] private Image rightSpeakerImage;

    [Header("List of texts spoken")]
    [SerializeField] private List<DialogueSpoken> spokenDialogue = new List<DialogueSpoken>();
    [SerializeField] private int currentDialogueIndex = 0;
    [SerializeField] private Text dialogueTextObject;


    private void Start() {
        leftSpeakerImage.enabled = false;
        rightSpeakerImage.enabled = false;
        if (playsOnStart) {
            StartFirstDialogue();
        }
    }

    private void StartFirstDialogue() {
        if (spokenDialogue.Count > 0) {
            DisplayCurrentInfo();
        }
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            if (currentDialogueIndex < spokenDialogue.Count - 1) {
                currentDialogueIndex++;
                DisplayCurrentInfo();

            } else {
                Destroy(gameObject);
            }
        }
    }

    private void DisplayCurrentInfo() {
        // getting text to show up
        dialogueTextObject.text = spokenDialogue[currentDialogueIndex].textSpoken;

        // getting images to show up
        List<DialogueSpeaker> dialogueSide = leftSideSpeakers;
        if (spokenDialogue[currentDialogueIndex].rightSide) {
            dialogueSide = rightSideSpeakers;
        }
        int speakerIndex = spokenDialogue[currentDialogueIndex].speakerIndex;

        if (speakerIndex < dialogueSide.Count) {
            if (spokenDialogue[currentDialogueIndex].rightSide) {
                rightSpeakerImage.enabled = true;
                rightSpeakerImage.sprite = dialogueSide[speakerIndex].characterSprite;
            } else {
                leftSpeakerImage.enabled = true;
                leftSpeakerImage.sprite = dialogueSide[speakerIndex].characterSprite;
            }
        }
    }

    // private IEnumerator FadeInImage(Image image, Color colorFade) {
    //     yield return null;
    // }

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
        public int speakerIndex;
        public bool rightSide;

    }
}
