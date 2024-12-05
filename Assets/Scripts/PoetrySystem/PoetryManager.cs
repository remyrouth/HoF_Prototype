using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class PoetryManager : MonoBehaviour
{
    [SerializeField] private Text poemDisplay;
    [SerializeField] private List<Decision> options = new();
    
    [SerializeField] private string poem;
            
    /** Poetry Key
     * Poetry string input format:
     * word word [] word word word word
     * 
     * words = standard words
     * [ = start button set
     * ] = end button set
     * 
     */

    #region Start

    // Initial set up. All decisions are disabled so that only text is displayed and Update is in effect. 
    void Start()
    {
        Debug.Log("Starting game. Buttons under each decision should be disabled.");
        DisableOptions();
    }

    #endregion
    
    #region TextEffects

    // Typewriter and scroll effect. 
    
    // Typewriter effect values.
    private float timer = 0; // represents the timer which indicates when characters should appear.
    private int charInd = 0; // represents what index of characters is displayed.
    private int decisionInd = 0; // Which decision set is being 
    private float timePerChar = 0.1f; // Speed at which chars appear.

    // Scroll values.
    private int scrollDelta = 12; // The distance the text is moved.
    private bool shifting = false; // Does the text move up.
    private int countEnts = 0;

    
    // Update the display.
    private void Update()
    {
        if (string.IsNullOrEmpty(poem))
        {
            Debug.Log("No poem provided. Using Text.text.");
            poem = poemDisplay.text;
        }

        if (shifting == false)
        {
            if (countEnts == 10)
            {
                shifting = true;
            }
        }

        timer -= Time.deltaTime;
        if (timer <= 0) 
        {
            if (charInd >= poem.Length - 1)
            {
                poemDisplay.text = poem;
                Time.timeScale = 0;
            }
            else
            {
                timer += timePerChar;
                charInd++;
                
                if (poem[charInd].ToString() == "[")
                {
                    Time.timeScale = 0;
                    
                    UpdateButtonDisplays(decisionInd);
                                
                    // update char ind and display word.
                    
                    // remove [], will be replaced by selection.
                    poem = poem.Remove(charInd, 2);
                    
                    decisionInd++;
                }

                if (poem[charInd].ToString() == "\n")
                {
                    if (shifting)
                    {
                        // Moves the text upwards. 
                        // If a background is added, needs to move in tandem w text lines, and should be synched here.
                        Vector3 pos = poemDisplay.transform.position;
                        pos.y += scrollDelta;

                        poemDisplay.transform.position = pos;
                    }
                    else
                    {
                        countEnts++;
                    }
                }
                            
                poemDisplay.text = poem.Substring(0, charInd);
            }
            
            
        }

    }

    #endregion


    #region DecisionsAndChoices

    // Representing Each decision set.
    [Serializable]
    public class Decision
    {
        [SerializeField] private List<Button> choices = new ();
        
        public List<Button> GetChoices()
        {
            return choices;
        }
        
    }
    
    
    
    #endregion 
    
    
    
    // Button Selection

    #region Button
    
    private void UpdateButtonDisplays(int buttonSet)
    {
        
        // Display the options within the button set
        Debug.Log("Slowly Implementing UpdateButtonDisplays to debug.");
        Debug.Log("Update button displays currently sets buttons active. Does not place.");

        // activate buttons in this decision set
        Decision decision = options[buttonSet];
        int screen = 400;
        float choicesX = screen / decision.GetChoices().Count;

        int choicesInd = 1;
        
        foreach (Button b in decision.GetChoices())
        {
            Vector3 pos = new Vector3(0.0f, 203.0f, 0.0f);// b.gameObject.transform.position;
            // Alters the x coord of button to be aligned to poetry and then evenly spaced. 
            // buttons currently left aligned.
            pos.x = 350 + 100 * (choicesInd - 1); // (.5f * pos.x) + (choicesInd * choicesX);
            
            b.transform.position = pos;
            b.gameObject.SetActive(true);
            
            choicesInd++;
        }
        

    }
    
    // Disabling All buttons
    private void DisableOptions()
    {
        foreach (Decision decision in options)
        {
            ResetChoices(decision);
        }

        Time.timeScale = 1;
    }

    // Disabling Buttons on the decision basis. 
    private void ResetChoices(Decision decision)
    {
        foreach (Button choice in decision.GetChoices())
        {
            choice.gameObject.SetActive(false);
        }
    }

    public void UpdateDisplay(string display)
    {
        Debug.Log("Update display has been called. Button properly clicked.");
        if (string.IsNullOrEmpty(display))
        {
            //
        }
        
        // inserting the word into the poem. 
        poem = poem.Insert(charInd, display);
            
        // adjust charind.
        charInd += display.Length;
            
        DisableOptions();
    }
    
    #endregion
    
}