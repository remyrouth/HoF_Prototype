using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MechSaveFileInteractor : MonoBehaviour
{
    public TeamBuilder defaultAvailableUnits;

    // File path to save the data
    private string filePath = "Assets/SaveFiles/MechSaveFile1.txt"; // You can set the path from Unity Inspector
    public bool restartSaveFile = false;

    private void Start()
    {
        if (restartSaveFile) {
            ClearFileContents();
            for (int i = 0; i < defaultAvailableUnits.MechLength(); i++)
            {
                LogMechStatsToFile(defaultAvailableUnits.GetMech(i));
            }
        }

    }

    // Call this method to log the mech stats
    public void LogMechStatsToFile(MechStats individualMech)
    {
        // Prepare the string with the data
        string dataToWrite = $"Mech Name: {individualMech.GetMechName()} [ " +
                             $"Mech Health Upgrade Count: {0} " +
                             $"Max Clarity Upgrade Count: {0} ]\n";

        // Write the data to the file
        WriteToFile(dataToWrite);
    }

    // Function to handle writing to a file
    private void WriteToFile(string content)
    {
        try
        {
            // Append the content to the specified file
            File.AppendAllText(filePath, content);
            Debug.Log("Mech stats logged to file: " + filePath);
        }
        catch (IOException e)
        {
            Debug.LogError("Failed to write to file: " + e.Message);
        }
    }
    public void ClearFileContents()
    {
        try
        {
            File.WriteAllText(filePath, string.Empty); // Overwrites the file with an empty string
            Debug.Log("File contents deleted: " + filePath);
        }
        catch (IOException e)
        {
            Debug.LogError("Failed to clear file: " + e.Message);
        }
    }
    private string ExtractMechName(string line)
    {
        int nameStartIndex = line.IndexOf("Mech Name: ") + "Mech Name: ".Length;
        int nameEndIndex = line.IndexOf(" [", nameStartIndex);

        if (nameStartIndex >= 0 && nameEndIndex > nameStartIndex)
        {
            return line.Substring(nameStartIndex, nameEndIndex - nameStartIndex);
        }
        return null; // Return null if the name cannot be extracted
    }
    private int ExtractMechHealth(string line)
    {
        int healthStartIndex = line.IndexOf("Mech Health Upgrade Count: ") + "Mech Health Upgrade Count: ".Length;
        int healthEndIndex = line.IndexOf(" Max Clarity Upgrade Count:", healthStartIndex);

        if (healthStartIndex >= 0 && healthEndIndex > healthStartIndex)
        {
            string healthStr = line.Substring(healthStartIndex, healthEndIndex - healthStartIndex).Trim();
            if (int.TryParse(healthStr, out int mechHealth))
            {
                return mechHealth;
            }
        }
        return -1; // Return -1 if health cannot be parsed
    }
    private bool CheckForFileExistence() {
        return File.Exists(filePath);
    }
    private int ExtractMechMaxClarity(string line)
    {
        int clarityStartIndex = line.IndexOf("Max Clarity Upgrade Count: ") + "Max Clarity Upgrade Count: ".Length;
        int clarityEndIndex = line.IndexOf(" ]", clarityStartIndex);

        if (clarityStartIndex >= 0 && clarityEndIndex > clarityStartIndex)
        {
            string clarityStr = line.Substring(clarityStartIndex, clarityEndIndex - clarityStartIndex).Trim();
            if (int.TryParse(clarityStr, out int mechMaxClarity))
            {
                return mechMaxClarity;
            }
        }
        return -1; // Return -1 if max clarity cannot be parsed
    }

    private MechStats GetSpecificMech(string mechName) {
        MechStats foundMech = null;
        foreach(MechStats mech in defaultAvailableUnits.mechs) {
            if (mech.GetMechName() == mechName) {
                foundMech = mech;
            }
        }

        return foundMech;
    }

    // called by upgrade mech controller script
    public List<UpgradeMechController.UpgradableMechUnit> ExtractMechsFromFile()
    {
        Debug.Log("Extraction called from save file");
        List<UpgradeMechController.UpgradableMechUnit> extractedMechs = new List<UpgradeMechController.UpgradableMechUnit>();
        if (!CheckForFileExistence()) {
            return extractedMechs;
        }

        // Read each line from the file
        string[] lines = File.ReadAllLines(filePath);
        foreach (string line in lines)
        {
            string mechName = ExtractMechName(line);
            int mechHealth = ExtractMechHealth(line);
            int mechMaxClarity = ExtractMechMaxClarity(line);
            if (mechName != null && mechHealth != -1 && mechMaxClarity != -1)
            {
                // Debug.Log($"Mech Name: {mechName}, Mech Health: {mechHealth}, Max Clarity: {mechMaxClarity}");
                MechStats mech = GetSpecificMech(mechName);
                if (mech != null) {
                    UpgradeMechController.UpgradableMechUnit mechUnit = new UpgradeMechController.UpgradableMechUnit();
                    mechUnit.mechBaseModel = mech;
                    mechUnit.maxHealthUpgradeCount = mechHealth;
                    mechUnit.maxClarityUpgradeCount = mechMaxClarity;

                    extractedMechs.Add(mechUnit);
                    // extractedMechs.Add(mech);
                } else {
                    Debug.LogWarning("Could not find mech named: " + mechName);
                }

            }
            else
            {
                Debug.LogWarning("Failed to parse line: " + line + " for mech: " + mechName);
            }
        }

        return extractedMechs;
    }

}
