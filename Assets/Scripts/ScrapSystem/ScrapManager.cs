using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ScrapManager : MonoBehaviour
{
    [SerializeField] private Dictionary<GameObject, int> _scrapAvailable;
    private string scrapFilePath = "Assets/SaveFiles";

    // Start is called before the first frame update
    void Start()
    {
        WriteToFile();
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: update the file
    }

    // Sums the total currency available
    private int GetTotalCurrency()
    {
        int total = 0;
        
        foreach (var scrap in _scrapAvailable)
        {
            total += scrap.Value;
        }
        
        return total;
    }

    // Tries to get the scrap value of a mech
    private int GetMechCurrency(GameObject mech)
    {
        if (_scrapAvailable.TryGetValue(mech, out int mechValue))
        {
            return mechValue;
        }
        else
        {
            Debug.LogError("No mech found");
            return -1;
        }
    }

    // Updates the dictionary with a new mech and its corresponding scrap value
    private void UpdateScrapAvailable(GameObject mech, int mechValue)
    {
        _scrapAvailable.Add(mech, mechValue);
        
        // TODO: update file 
    }

    // Removes a mech from the scrap available
    private void RemoveMech(GameObject mech)
    {
        _scrapAvailable.Remove(mech);
        
        // TODO: update file
    }
    
    // TODO: write to a file with the current currency available
    private void WriteToFile()
    {
        string path = "Assets/SaveFiles/ScrapSaveFile.txt";
        
        Debug.Log("dataPath: " + Application.dataPath);
        
        if (!File.Exists(path))
        {
            string text = "";
            
            if (_scrapAvailable.Count == 0)
            {
                text += "Hello there you're reading my text";
            }
    
            foreach (var pair in _scrapAvailable)
            {
                text += "Mech: " + pair.Key + ", Scrap value: " + pair.Value + Environment.NewLine;
            }
            
            File.WriteAllText(path, text, Encoding.UTF8);
        }
    }
    
    
    // TODO: read from a file once the game starts
    private void ReadFromFile()
    {
        
    }
}
