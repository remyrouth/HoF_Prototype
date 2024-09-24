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

    [Header("Speaker Indices Currently On")]
    [SerializeField] private int currentLeftSpeakerIndex = -1;
    [SerializeField] private int currentRightSpeakerIndex = -1;


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
        StartCoroutine(FadeInText(dialogueTextObject, 0.5f));

        ChangeSpeakerImage();
    }

    private IEnumerator FadeInImage(Image image, Sprite newSprite, float duration)
    {
        Color oldColor = image.color;
        float counter = 0;

        //fade out
        while(counter < duration)
        {
            counter += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, counter / duration);
            image.color = new Color(oldColor.r, oldColor.g, oldColor.b, alpha);
            yield return null;
        }

        image.sprite = newSprite;

        //fade in
        while(counter < duration)
        {
            counter += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, counter / duration);
            image.color = new Color(oldColor.r, oldColor.g, oldColor.b, alpha);
            yield return null;
        }

        
    }

    private IEnumerator FadeInText(Text text, float duration)
    {
        Color oldColor = text.color;
        float counter = 0;

        //fade out
        while(counter < duration)
        {
            counter += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, counter / duration);
            text.color = new Color(oldColor.r, oldColor.g, oldColor.b, alpha);
            yield return null;
        }

        text.text = spokenDialogue[currentDialogueIndex].textSpoken;

        //fade in
        while(counter < duration)
        {
            counter += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, counter / duration);
            text.color = new Color(oldColor.r, oldColor.g, oldColor.b, alpha);
            yield return null; 
        }
    }

    private void ChangeSpeakerImage()
    {
        List<DialogueSpeaker> dialogueSide = leftSideSpeakers;
        bool isRightSide = spokenDialogue[currentDialogueIndex].rightSide;
        if (isRightSide)
        {
            dialogueSide = rightSideSpeakers;
        }


        int speakerIndex = spokenDialogue[currentDialogueIndex].speakerIndex;

        if (speakerIndex < dialogueSide.Count)
        {
            Sprite newSprite = dialogueSide[speakerIndex].characterSprite;
           
            if (spokenDialogue[currentDialogueIndex].rightSide)
            {
                if(spokenDialogue[currentDialogueIndex].speakerIndex == currentRightSpeakerIndex)
                {
                    StartCoroutine(FadeInImage(rightSpeakerImage, newSprite, 0.5f));
                }
                else
                {
                    rightSpeakerImage.sprite = newSprite;
                }
                
            }

            else
            {
                if (spokenDialogue[currentDialogueIndex].speakerIndex == currentRightSpeakerIndex)
                {
                    StartCoroutine(FadeInImage(leftSpeakerImage, newSprite, 0.5f));
                }
                else
                {
                    rightSpeakerImage.sprite = newSprite;
                }
            }
        }


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
        public int speakerIndex;
        public bool rightSide;

    }
}
