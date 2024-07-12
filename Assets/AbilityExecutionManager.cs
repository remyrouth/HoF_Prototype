using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityExecutionManager : MonoBehaviour
{
    public AbilityExecutionController ExecutionController;
    // inputs : (These are all the component that make an ability work)
    // Summons prefab
    // targeted tile
    // heals or damages players
    // min and max ranges
    // if it requires an entity on said tile
    // if it targets allies or enemies
    


    // returns true if the ability executed
    // WE ONLY CARE ABOUT PILOT AND MECH MUTABLE DATA HERE

    public bool InputAbilityInformationSources(MechStats.AbilityMechSlot ability, PlayerController character, GameObject tileTarget) {

        bool legal = CanExecuteAbilityLegally(ability, character, tileTarget);

        if (legal) {
            ExecutionController.execute();
        }

        return legal;

        // return false;
    }

    // ExecutionController just does the ability. We first
    // have to check if the ability is legal, using all the
    // mutable variables
    private bool CanExecuteAbilityLegally(MechStats.AbilityMechSlot ability, PlayerController character, GameObject tileTarget) {

        bool clarityCostCheck = character.currentClarityLevel >= ability.GetClarityCost();

        int minRange = Mathf.Max(ability.GetMinimumRange() - 1, 0);
        List<GameObject> subtractableTiles = character.GetAttackableTiles(minRange);
        List<GameObject> maxTiles = character.GetAttackableTiles(ability.GetMaximumRange());
        List<GameObject> targetableTiles = RemoveGameObjects(maxTiles, subtractableTiles);

        // we should not care if its an ally or enemy right now
        bool isInRangeCheck = targetableTiles.Contains(tileTarget);

        return false;
    }




    // Method to remove a list of GameObjects from another list of GameObjects
    public List<GameObject> RemoveGameObjects(List<GameObject> sourceList, List<GameObject> objectsToRemove)
    {
        // Create a new list to store the updated list
        List<GameObject> updatedList = new List<GameObject>(sourceList);

        // Iterate through the objectsToRemove list
        foreach (GameObject obj in objectsToRemove)
        {
            // If the object exists in the updatedList, remove it
            if (updatedList.Contains(obj))
            {
                updatedList.Remove(obj);
            }
        }

        // Return the updated list
        return updatedList;
    }





}
