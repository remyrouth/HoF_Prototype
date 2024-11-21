using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterStats", menuName = "Character/NewMech")]
public class MechStats : ScriptableObject
{
    // we can change the public fields to serialized private anyway,
    // its just a left over from prototyping that they're public. If
    // any problems, ask Remy for help
    [SerializeField] private string mechName; 
    public int mechHealth;
    public Sprite characterSprite;
    public int maximumClarity;
    [SerializeField] private GameObject mechGFXPrefab;
    [SerializeField] private Sound mechIdleSFX;
    public MechClass mechAbilityClass = MechClass.StandardSoldier;

    public AbilityMechSlot AbilitySlot1 = new AbilityMechSlot();
    public AbilityMechSlot AbilitySlot2 = new AbilityMechSlot();
    public AbilityMechSlot AbilitySlot3 = new AbilityMechSlot();

    public Sound GetMechIdleSFX() {
        return mechIdleSFX;
    }

    public string GetMechName() {
        return mechName;
    }

    public Sprite GetMechSprite()
    {
        return characterSprite;
    }

    public GameObject GetMechGFXPrefab() {
        return mechGFXPrefab;
    } 

    public int GetMechHealth() {
        return mechHealth;
    }

    public int GetMechMaxClarity() {
        return maximumClarity;
    }

    public MechClass GetMechType() {
        return mechAbilityClass;
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

        public void SetValues(int newPower, int newClarityCost, int newMinimumRange, int newMaximumRange) {
            intPower = newPower;
            clarityCost = newClarityCost;
            minmumRange = newMinimumRange;
            maximumRange = newMaximumRange;
        }


        // Getters for Ability class properties
        public AbilityType GetAbilityType() {
            return type;
        }

        public void SetAbilityType(AbilityType newType) {
            type = newType;
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
                case AbilityType.Laser:
                    return "Shoots mech targeting lasers as a basic attack";
                case AbilityType.Ballistic:
                    return "Shoots pilot targeting ballistics as a basic attack";
                case AbilityType.Combo:
                    return "Uses a combo of lasers and ballistics doing 50%\ndamage to both pilot and mech as a basic attack";
                case AbilityType.Heal:
                    return "Healing ability boosts both a mechs and pilots HP";
                case AbilityType.RocketSummon:
                    return "Summons a rocket which targets the nearest enemy mech, damaging mechs";
                case AbilityType.TeleportToTile:
                    return "Allows your unit to teleport to a tile instantly";
                case AbilityType.ConvertEnemy:
                    return "Converts a unit to the opposing side";
                case AbilityType.LightningStrike:
                    return "Strikes the enemy with lightning, frying the mechs components";
                case AbilityType.Artillery:
                    return "Fires an artillery charge at a target enemy, damaging the mech";
                case AbilityType.AllyDefenseBoost:
                    return "Boost the defense of an ally. A temporary health boost equivalent";
                default:
                    return "Ability Description not \nwritten yet. \nLook Into MechStats.cs file...";
            }
        }

    }
    public enum AbilityType {
        None,
        Laser,
        Ballistic,
        Combo,
        Heal,
        RocketSummon,
        TeleportToTile,
        RockSummon,
        ConvertEnemy,
        LightningStrike,
        Artillery,
        AllyDefenseBoost
    }


}
