using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Generic ability strategy
public class GenericAbilityStrategy : IAbilityStrategy
{
    protected AbilityTraits _traits;

    public GenericAbilityStrategy(AbilityTraits traits)
    {
        _traits = traits;
    }

    public virtual bool Execute(MechStats.AbilityMechSlot ability, PlayerController character, GameObject tileTarget)
    {
        // Debug.Log("Calling Execute command");
        if (!CheckRequirements(ability, character, tileTarget)) {
            // Debug.Log("was not a legal use of ability");
            return false;
        }

        ApplyHealthEffect(ability, character, tileTarget);
        ApplyMovementEffect(character, tileTarget);
        SummonPrefab(character, tileTarget);

        // Debug.Log("returning true for Execute command");
        return true;
    }

    protected virtual void ApplyMovementEffect(PlayerController character, GameObject tileTarget) {
        switch(_traits.MovementEffect) {
            case AbilityRules.MovementImpactType.TeleportToTile:
                    Vector3 newPos = tileTarget.transform.position;
                    character.gameObject.transform.position = new Vector3(newPos.x, character.gameObject.transform.position.y, newPos.z);
                    character.MoveToTile(tileTarget);
                break;
            case AbilityRules.MovementImpactType.PullTowardsTarget:
                    // not yet implemented
                break;
            case AbilityRules.MovementImpactType.PushAwayFromTarget:
                    // not yet implemented
                break;
            case AbilityRules.MovementImpactType.None:
                    // we do nothing here, we skip
                break;
            default:
                    Debug.LogError("Switch statement wasn't prepped for this new DamageType type\n"+
                    " which is DamageType:" + _traits.MovementEffect.ToString());
                // return false;
                break;
        }
    }

    protected virtual void SummonPrefab(PlayerController character, GameObject tileTarget)
    {
        if (_traits.PrefabToSummon == null)
        {
            // Debug.Log("Null prefab given");
            return; // No prefab to summon
        }

        Vector3 sourcePos = character.FindClosestTile(character.gameObject.transform.position).transform.position;
        Quaternion sourceRotation = character.gameObject.transform.rotation;
        Vector3 targetPos = tileTarget.transform.position;

        switch (_traits.PrefabPlacementMethod)
        {
            case AbilityRules.PrefabSummoningPlacement.AtSource:
                InstantiatePrefab(sourcePos, sourceRotation);
                break;

            case AbilityRules.PrefabSummoningPlacement.AtTarget:
                // Debug.Log("target switch called");
                InstantiatePrefab(targetPos, sourceRotation);
                break;

            case AbilityRules.PrefabSummoningPlacement.AtBothSourceAndTarget:
                InstantiatePrefab(sourcePos, sourceRotation);
                InstantiatePrefab(targetPos, sourceRotation);
                break;

            case AbilityRules.PrefabSummoningPlacement.None:
                // Do nothing
                break;

            default:
                Debug.LogWarning($"Unhandled PrefabSummoningPlacement: {_traits.PrefabPlacementMethod}");
                break;
        }
    }

    private void InstantiatePrefab(Vector3 position, Quaternion rotation)
    {
        // Debug.Log("InstantiatePrefab method called");
        GameObject instance = Object.Instantiate(_traits.PrefabToSummon, position, rotation);
        
        // Optional: You can add additional setup for the instantiated prefab here
        // Example: Set the prefab to destroy itself after 5 seconds
        // Object.Destroy(instance, 5f);
    }

    protected virtual void ApplyHealthEffect(MechStats.AbilityMechSlot slot, PlayerController character, GameObject tileTarget) {
        // Debug.Log("Calling ApplyHealthEffect command");
        GameObject matchingObject = character.FindMatchingObjectToTile(tileTarget);
        int powerInt = 0;

        // Determine target type and apply effects accordingly
        if (matchingObject != null) {
            var pcTarget = matchingObject.GetComponent<PlayerController>();
            var obstTarget = matchingObject.GetComponent<ObstacleController>();

            switch (_traits.HealthEffect) {
                case AbilityRules.DamageType.Heals:
                    powerInt = -slot.GetIntPower();
                    ApplyDamage(pcTarget, obstTarget, powerInt);
                    break;
                case AbilityRules.DamageType.Damages:
                    // Debug.Log("Applied Damge");
                    powerInt = slot.GetIntPower();
                    ApplyDamage(pcTarget, obstTarget, powerInt);
                    break;
                case AbilityRules.DamageType.None:
                    // Do nothing
                    break;
                default:
                    Debug.LogError("Switch statement wasn't prepped for this new DamageType type which is DamageType: " + _traits.HealthEffect.ToString());
                    break;
            }
        }
    }

    private void ApplyDamage(PlayerController pcTarget, ObstacleController obstTarget, int powerInt) {
        if (pcTarget != null) {
            switch (_traits.HealthTarget) {
                case AbilityRules.EntityHealthTargetType.Pilot:
                    // Debug.Log("ApplyDamage method called");
                    pcTarget.TakeDamage(powerInt, false);
                    break;
                case AbilityRules.EntityHealthTargetType.Mech:
                    pcTarget.TakeDamage(powerInt, true);
                    break;
                case AbilityRules.EntityHealthTargetType.Both:
                    pcTarget.TakeDamage(powerInt/2, true);
                    pcTarget.TakeDamage(powerInt/2, false);
                    break;
                case AbilityRules.EntityHealthTargetType.None:
                    // Do nothing
                    break;
            }
        } else if (obstTarget != null) {
            obstTarget.TakeDamage(powerInt);
        }
    }








    // Legality Methods
    protected virtual bool CheckRequirements(MechStats.AbilityMechSlot ability, PlayerController character, GameObject tileTarget) {
        bool clarityCheck = ClarityCheck(ability, character);
        bool rangeCheck = RangeCheck(ability, character, tileTarget);
        bool sightCheck = CanSeeEachOther(character, tileTarget);

        // Debug.Log("clarityCheck: " + clarityCheck + " | rangeCheck: " + rangeCheck + " | sightCheck: " + sightCheck);
        return clarityCheck && rangeCheck && sightCheck;
    }

    private bool CanSeeEachOther(PlayerController character, GameObject tileTarget) {
        if (_traits.RequiresLineOfSight) {
            Vector3 direction = tileTarget.transform.position - character.gameObject.transform.position;
            float distance = direction.magnitude;

            Ray ray = new Ray(character.gameObject.transform.position, direction);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, distance))
            {
                return hit.transform.gameObject == tileTarget;
            }

            return false;
        }

        return true;
    }

    private bool ClarityCheck(MechStats.AbilityMechSlot ability, PlayerController character) {
        return character.currentClarityLevel >= ability.GetClarityCost();
    }

    private bool RangeCheck(MechStats.AbilityMechSlot ability, PlayerController character, GameObject tileTarget) {
        int minRange = Mathf.Max(ability.GetMinimumRange(), 0);
        List<GameObject> subtractableTiles = character.GetAttackableTiles(minRange);
        List<GameObject> maxTiles = character.GetAttackableTiles(ability.GetMaximumRange());
        List<GameObject> targetableTiles = RemoveGameObjects(maxTiles, subtractableTiles);

        // we should not care if its an ally or enemy right now
        return maxTiles.Contains(tileTarget);
    }

    private List<GameObject> RemoveGameObjects(List<GameObject> sourceList, List<GameObject> objectsToRemove)
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