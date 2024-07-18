using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityExecutionManager : MonoBehaviour
{

    [Header("Healing Ability")]
    public AbilityRules HealingAbilityRules =  new AbilityRules(MechStats.AbilityType.Heal,
        AbilityRules.DamageType.Heals,
        AbilityRules.ScalingType.Singular,
        AbilityRules.TileTargetType.Ally, 
        AbilityRules.PrefabSummoningPlacement.None, 
        AbilityRules.MovementImpactType.None, true);

    [Header("Lightning Strike Ability")]
    public AbilityRules LightningStrikeAbilityRules =  new AbilityRules(MechStats.AbilityType.LightningStrike,
        AbilityRules.DamageType.Damages,
        AbilityRules.ScalingType.Singular,
        AbilityRules.TileTargetType.Enemy, 
        AbilityRules.PrefabSummoningPlacement.None, 
        AbilityRules.MovementImpactType.None, false);

    [Header("Teleport Ability")]
    public AbilityRules TeleportAbilityRules =  new AbilityRules(MechStats.AbilityType.Teleport,
        AbilityRules.DamageType.None,
        AbilityRules.ScalingType.Singular,
        AbilityRules.TileTargetType.Empty, 
        AbilityRules.PrefabSummoningPlacement.None, 
        AbilityRules.MovementImpactType.TeleportToTile, false);

    [Header("Summon Rock Ability")]
    public AbilityRules RockSummonAbilityRules =  new AbilityRules(MechStats.AbilityType.RocketSummon,
        AbilityRules.DamageType.None,
        AbilityRules.ScalingType.Singular,
        AbilityRules.TileTargetType.Empty, 
        AbilityRules.PrefabSummoningPlacement.AtTarget, 
        AbilityRules.MovementImpactType.None, false);


    // Ability method inputs : int power, target tile, min / max ranges, current clarity


    // returns true if the ability executed
    public bool InputAbilityInformationSources(MechStats.AbilityMechSlot ability, PlayerController character, GameObject tileTarget) {
        // Debug.Log("InputAbilityInformationSources ability used, " + ability.GetAbilityType().ToString());
        bool legal = ClarityCheck(ability, character) && RangeCheck(ability, character, tileTarget);
        // Debug.Log("Clarity Check = " + ClarityCheck(ability, character));
        // Debug.Log("RangeCheck Check = " + RangeCheck(ability, character, tileTarget));

        // activate ability here
        if (legal) {
            bool abilityUseCheck = ActivateAbility(character, ability, tileTarget);
            // Debug.Log("ActivateAbility(character, ability, tileTarget) = " + abilityUseCheck);
            return abilityUseCheck;
        }

        return legal;
    }

    private bool ActivateAbility(PlayerController character, MechStats.AbilityMechSlot abilityClass, GameObject tileTarget) {
        // Debug.Log("Actived the ActivateAbility method, which calls the UseAbility method on class");
        MechStats.AbilityType abilityType = abilityClass.GetAbilityType();
        // Debug.Log("Type: : " + abilityType.ToString());
        switch(abilityType) {
            case MechStats.AbilityType.None:
                    return false;
            case MechStats.AbilityType.Heal:
                // bool healPassCheck = HealingAbilityRules.UseAbility(character, abilityClass, tileTarget);
                return HealingAbilityRules.UseAbility(character, abilityClass, tileTarget);
            case MechStats.AbilityType.LightningStrike:
                return LightningStrikeAbilityRules.UseAbility(character, abilityClass, tileTarget);
            case MechStats.AbilityType.Teleport:
                    // Debug.Log("Teleport worked");
                return TeleportAbilityRules.UseAbility(character, abilityClass, tileTarget);
            case MechStats.AbilityType.RockSummon:
                bool rockPassCheck = RockSummonAbilityRules.UseAbility(character, abilityClass, tileTarget);
                // Debug.Log("rockPassCheck: " + rockPassCheck);
                // return RockSummonAbilityRules.UseAbility(character, abilityClass, tileTarget);
                return rockPassCheck;
            default:
                // Debug.Log("Went To Default");
                return false;
                // break;
        }

        // return false;
    }






    // Legality Methods 
    private bool ClarityCheck(MechStats.AbilityMechSlot ability, PlayerController character) {
        return character.currentClarityLevel >= ability.GetClarityCost();
    }

    private bool RangeCheck(MechStats.AbilityMechSlot ability, PlayerController character, GameObject tileTarget) {
        int minRange = Mathf.Max(ability.GetMinimumRange(), 0);
        List<GameObject> subtractableTiles = character.GetAttackableTiles(minRange);
        List<GameObject> maxTiles = character.GetAttackableTiles(ability.GetMaximumRange());
        List<GameObject> targetableTiles = RemoveGameObjects(maxTiles, subtractableTiles);

        // we should not care if its an ally or enemy right now
        // return targetableTiles.Contains(tileTarget);
        return maxTiles.Contains(tileTarget);
    }

    // Helper Methods
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

    bool CanSeeEachOther(Transform objA, Transform objB)
    {
        Vector3 direction = objB.position - objA.position;
        float distance = direction.magnitude;

        Ray ray = new Ray(objA.position, direction);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distance))
        {
            // Check if the object hit by the ray is the intended target
            if (hit.transform == objB)
            {
                return true;
            }
        }

        return false;
    }

    [System.Serializable]
    public class AbilityRules {
        private MechStats.AbilityType abilityChosen;
        public GameObject prefabToSummon;
        public DamageType healthEffect;
        public ScalingType scalingMethod;
        public TileTargetType tileTargetingMethod;
        public PrefabSummoningPlacement prefabPlacementMethod;
        public MovementImpactType movementEffect;
        public bool requiresLineOfSight = true;

        // Constructor
        public AbilityRules(MechStats.AbilityType abilityChosen, DamageType healthEffect,
            ScalingType scalingMethod, TileTargetType tileTargetingMethod, 
            PrefabSummoningPlacement prefabPlacementMethod, MovementImpactType movementEffect, bool requiresLineOfSight) 
        {
            // this.prefabToSummon = prefabToSummon;
            this.abilityChosen = abilityChosen;
            this.healthEffect = healthEffect;
            this.scalingMethod = scalingMethod;
            this.tileTargetingMethod = tileTargetingMethod;
            this.prefabPlacementMethod = prefabPlacementMethod;
            this.movementEffect = movementEffect;
            this.requiresLineOfSight = requiresLineOfSight;

        }

        public bool UseAbility(PlayerController character, MechStats.AbilityMechSlot slot, GameObject tileTarget) {
            if (character == null || slot == null || tileTarget == null) {
                Debug.Log("MISSING");
            }
            
            // Debug.Log("Actived the UseAbility method");
            bool tileUseCheck = CheckTileUsage(character, tileTarget);
            bool sightCheck = !requiresLineOfSight || 
                (requiresLineOfSight && CanSeeEachOther(character.gameObject.transform, tileTarget.transform));
            // Debug.Log("(tileUseCheck && sightCheck): " + (tileUseCheck && sightCheck)); 

            if (tileUseCheck && sightCheck) {
                // Debug.Log("Passed the CheckTileUsage method check");
                UseHealthEffect(character, slot, tileTarget);
                SummonPrefabAtLocation(character, tileTarget);
                UseMovementEffect(character, slot, tileTarget);
                return true;
            } else {
                return false;
            }

        }

        private void SummonPrefabAtLocation(PlayerController character, GameObject tileTarget) {
            if (prefabPlacementMethod != PrefabSummoningPlacement.None) {
                if (prefabToSummon != null) {
                    Vector3 sourcePos = character.FindClosestTile(character.gameObject.transform.position).transform.position;
                    Quaternion sourceRotation = character.gameObject.transform.rotation;
                    Vector3 targetPos = tileTarget.transform.position;

                    switch(prefabPlacementMethod) {

                        case PrefabSummoningPlacement.AtSource:
                            Instantiate(prefabToSummon, sourcePos, sourceRotation);

                            break;
                        case PrefabSummoningPlacement.AtTarget:
                            Instantiate(prefabToSummon, targetPos, sourceRotation);

                            break;
                        case PrefabSummoningPlacement.AtBothSourceAndTarget:
                            Instantiate(prefabToSummon, sourcePos, sourceRotation);
                            Instantiate(prefabToSummon, targetPos, sourceRotation);

                            break;
                        case PrefabSummoningPlacement.None:
                            // skip and do nothing

                            break;
                        default:
                                Debug.LogError("Switch statement wasn't prepped for this new PrefabSummoningPlacement type\n"+
                                " which is prefabPlacementMethod:" + prefabPlacementMethod.ToString());
                            break;
                    }
                } else {
                    Debug.LogError("Prefab Summon Object was not given to this class, it was meant to.\n" +
                    "Fix the method SummonPrefabAtLocation in the AbilityExecutionManager");
                }
            }
        }

        private void UseHealthEffect(PlayerController character, MechStats.AbilityMechSlot slot, GameObject tileTarget) {
            GameObject matchingObject = character.FindMatchingObjectToTile(tileTarget);
            PlayerController pcTarget = null;
            ObstacleController obstTarget = null;

            if (matchingObject != null) {
                pcTarget = matchingObject.GetComponent<PlayerController>();
                obstTarget = matchingObject.GetComponent<ObstacleController>();

            }
            int powerInt = 0;

            switch(healthEffect) {
                case DamageType.Heals:
                        powerInt = 0 - slot.GetIntPower();
                        if (pcTarget != null) {
                            pcTarget.TakeDamage(powerInt);
                        } else if (obstTarget != null) {
                            obstTarget.TakeDamage(powerInt);
                        }
                    break;
                case DamageType.Damages:
                        powerInt = slot.GetIntPower();
                        if (pcTarget != null) {
                            pcTarget.TakeDamage(powerInt);
                        } else if (obstTarget != null) {
                            obstTarget.TakeDamage(powerInt);
                        }
                    break;
                case DamageType.None:
                        // we do nothing here, we skip
                        // powerInt = 0;
                    break;
                default:
                        Debug.LogError("Switch statement wasn't prepped for this new DamageType type\n"+
                        " which is DamageType:" + healthEffect.ToString());
                    // return false;
                    break;
            }

            // if (pcTarget != null) {
            //     pcTarget.TakeDamage(powerInt);
            // } else if (obstTarget != null) {
            //     obstTarget.TakeDamage(powerInt);
            // }
        }

        private void UseMovementEffect(PlayerController character, MechStats.AbilityMechSlot slot, GameObject tileTarget) {
            switch(movementEffect) {
                case MovementImpactType.TeleportToTile:
                        Vector3 newPos = tileTarget.transform.position;
                        character.gameObject.transform.position = new Vector3(newPos.x, character.gameObject.transform.position.y, newPos.z);
                    break;
                case MovementImpactType.PullTowardsTarget:
                    break;
                case MovementImpactType.PushAwayFromTarget:
                    break;
                case MovementImpactType.None:
                        // we do nothing here, we skip
                    break;
                default:
                        Debug.LogError("Switch statement wasn't prepped for this new DamageType type\n"+
                        " which is DamageType:" + healthEffect.ToString());
                    // return false;
                    break;
            }
        }

        private bool CheckTileUsage(PlayerController character, GameObject tileTarget) {
            bool tileOccupied = character.IsTileOccupied(tileTarget);
            switch(tileTargetingMethod) {
                case TileTargetType.Empty:
                    // bool tileOccupied = character.IsTileOccupied(tileTarget);
                    return !tileOccupied;
                    // break;
                case TileTargetType.Enemy:
                    // Instantiate(prefabToSummon, sourcePos, sourceRotation);
                        if (tileOccupied) {
                            AIPlayerController aipcSource = character.gameObject.GetComponent<AIPlayerController>();
                            GameObject targetCharacter = character.FindMatchingObjectToTile(tileTarget);
                            AIPlayerController aipcTarget = targetCharacter.GetComponent<AIPlayerController>();
                            ObstacleController obstTarget = targetCharacter.GetComponent<ObstacleController>();

                            bool enemyCheck = ((aipcTarget == null && aipcSource != null) || (aipcTarget != null && aipcSource == null)) || obstTarget != null;
                            
                            if (enemyCheck)  {
                                return true;
                            }
                            // if Character.GameObject
                        }
                    return false;

                    // break;
                case TileTargetType.Ally:
                        if (tileOccupied) {
                            AIPlayerController aipcSource = character.gameObject.GetComponent<AIPlayerController>();
                            GameObject targetCharacter = character.FindMatchingObjectToTile(tileTarget);
                            AIPlayerController aipcTarget = targetCharacter.GetComponent<AIPlayerController>();
                            ObstacleController obstTarget = targetCharacter.GetComponent<ObstacleController>();
                            
                            if (((aipcTarget == null && aipcSource == null) ||
                             (aipcTarget != null && aipcSource != null)) || obstTarget != null) {
                                return true;
                            }
                            // if Character.GameObject
                        }
                    return false;

                    // break;
                default:
                        Debug.LogError("Switch statement wasn't prepped for this new TileTargetType type\n"+
                        " which is CheckTileUsage:" + prefabPlacementMethod.ToString());
                    return false;
                    // break;
            }
            
        }


        // Abilities
        // effect alliance --> convert
        // effect pathing --> Scare enemy
        // effect Status --> Give ally 30% chance to ignore next attack, refill main action, ability null field
        // effect physical position --> Shove / kick / groundpound / shockwave / teleport / Swap with friendly
        // effect health/damage --> lightning strike / 
        // effect map --> Bear trap / Pillar Barrier / FireWall / Fission Mine


        public enum DamageType {
            None,
            Damages,
            Heals
        }

        public enum ScalingType {
            Singular,
            FartherMeansGreaterEffect,
            CloserMeansGreaterEffect
        }

        public enum TileTargetType {
            Empty,
            Ally,
            Enemy
        }

        public enum PrefabSummoningPlacement {
            None,
            AtTarget,
            AtSource,
            AtBothSourceAndTarget
        }


        public enum MovementImpactType {
            None,
            PushAwayFromTarget,
            PullTowardsTarget,
            TeleportToTile
        }


        bool CanSeeEachOther(Transform objA, Transform objB)
        {
            Vector3 direction = objB.position - objA.position;
            float distance = direction.magnitude;

            Ray ray = new Ray(objA.position, direction);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, distance))
            {
                // Check if the object hit by the ray is the intended target
                if (hit.transform == objB)
                {
                    return true;
                }
            }

            return false;
        }

    }



}
