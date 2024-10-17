using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = System.Random;

public class CharacterDisplayController : MonoBehaviour
{
    [SerializeField] private Text pilotName;
    [SerializeField] private Text pilotFaction;
    [SerializeField] private Text causeOfDeath;
    [SerializeField] private Image pilotImage;
    [SerializeField] private Button dogtagPurchase;
    // [SerializeField] private GameObject backgroundScreen;
    private CharacterStats deceasedPilot;
    
    /*
     * Display notes:
     * Option One: 2 x 2 of displays, displaying 4 deceased at a time. Scroll buttons.
     * Option Two: 1 display, Left Scroll of deceased to select.
     *
     * Basic display:
     * Sprite display
     * Name of Pilot
     * Faction of pilot (colors added?)
     * Cause of Death (via list provided by design).
     *
     * Bug: Why are texts not running through? What is not hooked up correctly?
     * Could swap to TextMeshPro
     */
    
    public void DisplayDeceased(CharacterStats deceased)
    {
        if (deceased == null)
        {
            Debug.Log("deceased is null. Display default empty display.");
            return;
        }
        
        deceasedPilot = deceased;
        Debug.Log("Character Stats passed into DisplayDeceased: " + deceased.GetPilotName());
        // Name
        pilotName.text = NameDisplay(deceased.GetPilotName());
        // Faction
        pilotFaction.text = deceased.GetPilotFaction();
        // Cause of Death
        causeOfDeath.text = "Cause of death: " + CauseOfDeath(); // .CauseOfDeath()
        // Sprite
        pilotImage.sprite = deceased.GetCharacterSprite();
    }

    public void purchaseDogtag()
    {
        Debug.Log("purchase the dogtag of " + deceasedPilot.GetPilotName());
        Debug.Log("Currently not doing anything (purchasing dogtag). Currency must be implemented.");
        // Disable button after. May only be clicked once. 
        // Should affect the currency balance. Would need to save dogtag as possesion? 
        dogtagPurchase.interactable = false;
        Debug.Log("Dogtag button is now not interactable.");
    }
    
    // Formatting of Character Names (Last, First)
    private string NameDisplay(string pilotName)
    {
        string[] sections = pilotName.Split(" ");

        // Use this line if all names are inputted as First Last
        // string formatted = sections[1] + ", " + sections[0];
        if (sections.Count() > 2) { Debug.Log("More than 2 names provided instead of First Last"); }

        // If names are first last
        string testone = sections[1].ToCharArray()[0].ToString();
        string formatted = testone.ToUpper() + sections[1].Substring(1); 
        string test = sections[0].ToCharArray()[0].ToString();
        formatted += ", " + test.ToUpper() + sections[0].Substring(1);
        return formatted;
    }
    
    // Cause of Death
    private string CauseOfDeath()
    {
        List<string> deathCauses = new List<string>();
        deathCauses.Add("Toilet procedure error");
        deathCauses.Add("Struck by micrometeoroid");
        deathCauses.Add("Burned to death by exposure to direct sunlight");
        Random rnd = new Random(); 
        int rand = rnd.Next(0, deathCauses.Count); // rnd.Next(deathCauses.Count);
        return deathCauses[rand];
    }
    
}