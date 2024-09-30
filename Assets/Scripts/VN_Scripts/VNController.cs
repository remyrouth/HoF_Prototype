using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.Build;

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

    [Header("Fade in")] 
    [SerializeField] private float fadeInDuration = 3f;
    [SerializeField] private float fadeOutDuration = 2f;

    private bool leftImageHasFaded = false;
    private bool rightImageHasFaded = false;
    public Image textBox;
    public Text text;
    
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

            } else
            {
                StartCoroutine(FadeImage(rightSpeakerImage, false));
                StartCoroutine(FadeImage(leftSpeakerImage, false));
                
                if (textBox != null && text != null) 
                {
                    StartCoroutine(FadeOutText(text));
                    StartCoroutine(FadeImage(textBox, false));
                }
                
                Destroy(gameObject, fadeOutDuration);
            }
        }
    }

    private void DisplayCurrentInfo()
    {
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
                
                if (!rightImageHasFaded)
                {
                    StartCoroutine(FadeImage(rightSpeakerImage, true)); // image only fades in once enabled
                    rightImageHasFaded = true;
                }
            } else {
                leftSpeakerImage.enabled = true;
                leftSpeakerImage.sprite = dialogueSide[speakerIndex].characterSprite;

                if (!leftImageHasFaded)
                {
                    StartCoroutine(FadeImage(leftSpeakerImage, true)); // ditto
                    leftImageHasFaded = true;
                }
            }
        }
    }

    private IEnumerator FadeImage(Image image, bool fadeIn)
    {
        float timeElapsed = 0;
        
        float startVal = fadeIn ? 0 : 1; // startVal == 1 -> fades out. startVal == 0, fades in.
        float endVal = fadeIn ? 1 : 0;
        float duration = fadeIn ? fadeInDuration : fadeOutDuration;

        image.color = new Color(image.color.r, image.color.g, image.color.b, startVal);
        
        while (timeElapsed < fadeInDuration) // controls fade duration by slowly changing alpha of image
        {
            float alpha = Mathf.Lerp(startVal, endVal, timeElapsed / duration); 
            
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
            timeElapsed += Time.deltaTime; // keeps track of how much time has passed
            
            yield return null; 
        }
        
        image.color = new Color(image.color.r, image.color.g, image.color.b, endVal);
    }
    
    private IEnumerator FadeOutText(Text text)
    {
        // making sure image is opaque at first
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
        float timeElapsed = 0; 

        while (timeElapsed < fadeOutDuration) // controls fade duration by slowly decreasing alpha of text
        {
            float alpha = Mathf.Lerp(1, 0, timeElapsed / fadeOutDuration); 
            
            text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
            timeElapsed += Time.deltaTime; // keeps track of how much time has passed
            
            yield return null; 
        }
        
        // ensures text is at 0% alpha at the end of the loop
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
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
