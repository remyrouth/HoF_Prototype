using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Scrap Manager SO", menuName = "Scriptable Objects/ScrapManagerSO", order = 1)]
public class ScrapManager : ScriptableObject
{
    public int scrapAvailable;

    public int initialValue;
    public int currentValue;
    public bool resetScrap;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // Updates the dictionary with a new mech and its corresponding scrap value
    private void UpdateScrapAvailable(int mechValue)
    {
        scrapAvailable += mechValue;
        currentValue = scrapAvailable;
    }
    
    // Removes a mech from the scrap available
    private void RemoveMech(int mechValue)
    {
        if (scrapAvailable - mechValue < 0)
        {
            Console.Error.WriteLine("Not enough scrap available for this purchase");
        }
        else
        {
            scrapAvailable -= mechValue;
        }
    }

    private void resetScrapAmount()
    {
        if (resetScrap)
        {
            scrapAvailable = 0;
            currentValue = 0;
        }
    }
}
