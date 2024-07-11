using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterStats", menuName = "Character/NewMech")]
public class MechStats : ScriptableObject
{
    public int mechHealth;
    public Sprite characterSprite;
    public int maximumClarity;
    public MechClass mechAbilityClass = MechClass.StandardSoldier;

    public Ability AbilitySlot1 = new Ability();
    public Ability AbilitySlot2 = new Ability();
    public Ability AbilitySlot3 = new Ability();

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
    public class Ability {
        public AbilityType type = AbilityType.None;
        public int intPower;
        public int clarityCost = 2;
        public int minmumRange = 0;
        public int maximumRange = 1;
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

    public string RetreiveAbilityTypeDescription(Ability abilityClassInput) {
        AbilityType type = abilityClassInput.type;
        switch (type)
        {
            case AbilityType.None:
                return "Empty Ability Slot";
                // break;
            case AbilityType.Heal:
                return "Healing ability boosts both a mechs and pilots HP";
                // break;
            default:
                Debug.LogError("Ability Description not implemented yet. Look Into MechStats.cs file.\nThe method RetreiveAbilityTypeDescription is resonsible for this");
                return "Enum type is unknown";
                // break;
        }
        
    }

}
