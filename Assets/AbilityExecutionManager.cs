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
        AbilityRules.MovementImpactType.None);


    // Ability method inputs : int power, target tile, min / max ranges, current clarity


    // returns true if the ability executed
    public bool InputAbilityInformationSources(MechStats.AbilityMechSlot ability, PlayerController character, GameObject tileTarget) {

        bool legal = ClarityCheck(ability, character) && RangeCheck(ability, character, tileTarget);

        // activate ability here
        if (legal) {
            ActivateAbility(character, ability, tileTarget);
        }

        return legal;
    }

    private void ActivateAbility(PlayerController character, MechStats.AbilityMechSlot abilityClass, GameObject tileTarget) {
        MechStats.AbilityType abilityType = abilityClass.GetAbilityType();
        switch(abilityType) {
            case MechStats.AbilityType.None:
                    // return "Empty Ability Slot";
                    // 
                    HealingAbilityRules.UseAbility(character, abilityClass, tileTarget);
                break;
            case MechStats.AbilityType.Heal:

                break;
            default:

                break;
        }
    }






    // Legality Methods 
    private bool ClarityCheck(MechStats.AbilityMechSlot ability, PlayerController character) {
        return character.currentClarityLevel >= ability.GetClarityCost();
    }

    private bool RangeCheck(MechStats.AbilityMechSlot ability, PlayerController character, GameObject tileTarget) {
        int minRange = Mathf.Max(ability.GetMinimumRange() - 1, 0);
        List<GameObject> subtractableTiles = character.GetAttackableTiles(minRange);
        List<GameObject> maxTiles = character.GetAttackableTiles(ability.GetMaximumRange());
        List<GameObject> targetableTiles = RemoveGameObjects(maxTiles, subtractableTiles);

        // we should not care if its an ally or enemy right now
        return targetableTiles.Contains(tileTarget);
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

    [System.Serializable]
    public class AbilityRules {
        private MechStats.AbilityType abilityChosen;
        public GameObject prefabToSummon;
        public DamageType healthEffect;
        public ScalingType scalingMethod;
        public TileTargetType tileTargetingMethod;
        public PrefabSummoningPlacement prefabPlacementMethod;
        public MovementImpactType movementEffect;

        // Constructor
        public AbilityRules(MechStats.AbilityType abilityChosen, DamageType healthEffect,
            ScalingType scalingMethod, TileTargetType tileTargetingMethod, 
            PrefabSummoningPlacement prefabPlacementMethod, MovementImpactType movementEffect) 
        {
            this.abilityChosen = abilityChosen;
            this.healthEffect = healthEffect;
            this.scalingMethod = scalingMethod;
            this.tileTargetingMethod = tileTargetingMethod;
            this.prefabPlacementMethod = prefabPlacementMethod;
            this.movementEffect = movementEffect;

        }

        public void UseAbility(PlayerController character, MechStats.AbilityMechSlot slot, GameObject tileTarget) {
            if (CheckTileUsage(character, tileTarget)) {
                UseHealthEffect(slot, tileTarget);
                SummonPrefabAtLocation(character, tileTarget);
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

        private void UseHealthEffect(MechStats.AbilityMechSlot slot, GameObject tileTarget) {

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
                            
                            if ((aipcTarget == null && aipcSource != null) ||
                             (aipcTarget != null && aipcSource == null)) {
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
                            
                            if ((aipcTarget == null && aipcSource == null) ||
                             (aipcTarget != null && aipcSource != null)) {
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
        // effect Status --> Give ally 30% chance to ignore next attack, refill main action
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

    }



}
