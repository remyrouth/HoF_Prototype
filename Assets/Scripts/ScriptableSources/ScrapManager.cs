using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Scrap Manager SO", menuName = "Scriptable Objects/ScrapManagerSO", order = 1)]
public class ScrapManager : ScriptableObject
{
    [Tooltip("Current amount of scrap")] [SerializeField] private int scrapAvailable;
    [Tooltip("Scrap amount you'll start the game with")] [SerializeField] private int initialValue;
    public bool resetScrap;
    
    /*
     * script is called at the start of combat
     * - grab all enemy mechs, goes through each enemy at the end and sees if it has enough health so that the player can use it.
     *      - yes: if you have enough health to use the mech -> use as scrap OR mech
     *      - no: use it as scrap (if it's dead, you would just get zero scrap)
     *      --> should be done somewhere else, not the scrapmanager
     * 
     *      - when the method AddScrap is called, assign scrap based on some function of the health of the mechs
     *          - a mech's scrap value would have to be determined by a specific mech
     */
    
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnEnable()
    {
        if (resetScrap)
        {
            ResetScrapAmount();
        }
    }

    public void ResetScrapAmount()
    {
        if (resetScrap)
        {
            scrapAvailable = initialValue;
        }
    }

    public int GetScrapValue()
    {
        // TODO: returns how much a scrap a mech is worth 
        return 0;
    }

    public int GetScrapAvailable()
    {
        return scrapAvailable;
    }

    public void SetScrapAvailable(int newScrapAvailable)
    {
        scrapAvailable = newScrapAvailable;
    }
}
