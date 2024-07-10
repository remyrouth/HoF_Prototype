using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterStats", menuName = "Character/NewMech")]
public class MechStats : ScriptableObject
{
    public int mechHealth;
    public Sprite characterSprite;
    public int maximumClarity;
    public MechClass mechAbilityClass = MechClass.StandardSoldier;

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

    public class Ability {

        public enum AbilityType {

            
        }
    }
}
