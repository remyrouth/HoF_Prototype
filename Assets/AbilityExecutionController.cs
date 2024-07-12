using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAbilityController", menuName = "AbilityController/NewController")]

// Does not care about data of pilots or mechs. Simply executes the abilities.
// For example, we dont need minimum and maximum ranges, just the targeted tile.
// Everything hardcoded should be here, everything mutable should on the mechs and pilots

// must check if the non-mutable fields/variables allow for execution too
public class AbilityExecutionController : ScriptableObject
{
    [Header("Healing Ability")]
    public AbilityRules HealingAbilityRules =  new AbilityRules(MechStats.AbilityType.Heal);
    



    private bool ExecuteAbility(GameObject prefabSummon, bool damagesEntiy, GameObject targetedTile) {
        return false;
    }



    public bool execute() {
        return false;
    }



    [System.Serializable]
    public class AbilityRules {
        private MechStats.AbilityType abilityChosen;
        public GameObject prefabSummon;
        public bool damagesEntity;

        // Constructor
        public AbilityRules(MechStats.AbilityType abilityChosen)
        {
            this.abilityChosen = abilityChosen;
        }
    }

}


