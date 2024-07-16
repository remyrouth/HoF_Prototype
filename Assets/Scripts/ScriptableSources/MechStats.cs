using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterStats", menuName = "Character/NewMech")]
public class MechStats : ScriptableObject
{
    public int mechHealth;
    public Sprite characterSprite;
    public int maximumClarity;
    public MechClass mechAbilityClass = MechClass.StandardSoldier;

    public AbilityMechSlot AbilitySlot1 = new AbilityMechSlot();
    public AbilityMechSlot AbilitySlot2 = new AbilityMechSlot();
    public AbilityMechSlot AbilitySlot3 = new AbilityMechSlot();

    // returns true if we can successfully trigger the ability
    public bool UseAbility(GameObject TargetSquare) {
        return true;

    }

    public enum MechClass {
        StandardSoldier, // this has no abilities
        Titan,
        Striker,
        Sentinel,
        Ranger,
        Engineer,
        Pyro,
        Specter,
        Stormbringer,
        Gunner,
        Nomad
    }

    /// <Ability List>
    // Heal
    // Rocket summon
    // Teleport
    // Convert Enemy
    // Refill attack action
    // Sprint
    // Lightning strike (very far range)
    // Teleport enemy
    // Scare enemy
    // Give an ally 30% chance to ignore next damage taken
    /// <Ability List>

    [System.Serializable]
    public class AbilityMechSlot {
        [SerializeField]
        private AbilityType type = AbilityType.None;
        [SerializeField]
        private int intPower;
        [SerializeField]
        private int clarityCost = 2;
        [SerializeField]
        private int minmumRange = 1;
        [SerializeField]
        private int maximumRange = 1;


        // Getters for Ability class properties
        public AbilityType GetAbilityType() {
            return type;
        }

        public bool IsNotNoneType() {
            return type != AbilityType.None;
        }

        public int GetIntPower() {
            return intPower;
        }

        public int GetClarityCost() {
            return clarityCost;
        }

        public int GetMinimumRange() {
            return minmumRange;
        }

        public int GetMaximumRange() {
            return maximumRange;
        }

        // These methods are here because its neccessary for the character canvas 
        // controller script to access the mechs abilities.
        // So they should live here.
        // A ability mamanger script would just execute how these abilities are handled
        public string GetAbilityTypeDescription() {
            AbilityType classInput = type;
            switch (classInput)
            {
                case AbilityType.None:
                    return "Empty Ability Slot";
                    // break;
                case AbilityType.Heal:
                    return "Healing ability boosts both a mechs and pilots HP";
                    // break;
                default:
                    // Debug.LogError("Ability Description not implemented yet. Look Into MechStats.cs file.\nThe method RetreiveAbilityTypeDescription is resonsible for this");
                    // return "Enum type is unknown";
                    return "Ability Description not \nwritten yet. \nLook Into MechStats.cs file." + 
                    "\nThe method RetreiveAbilityTypeDescription is resonsible for this";
                    // break;
            }
        }

    }
    public enum AbilityType {
        None,
        Heal,
        RocketSummon,
        Teleport,
        ConvertEnemy,
        RefillAttackAction,
        Sprint,
        LightningStrike,
        ScareEnemy
    }


}
