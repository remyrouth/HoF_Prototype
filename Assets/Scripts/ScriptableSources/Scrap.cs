using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Scrap SO", menuName = "Scriptable Objects/ScrapSO", order = 1)]
public class Scrap : ScriptableObject
{
    [Tooltip("Current amount of scrap")] [SerializeField] private int scrapAvailable;
    [Tooltip("Amount of Scrap you'll start the game with")] [SerializeField] private int initialValue;
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
    
    private void OnEnable()
    {
        if (resetScrap)
        {
            if (resetScrap)
            {
                scrapAvailable = initialValue;
            }
        }
    }

    public int GetScrapAvailable()
    {
        return scrapAvailable;
    }

    public void SetScrapAvailable(int newValue)
    {
        scrapAvailable = newValue;
    }
}
