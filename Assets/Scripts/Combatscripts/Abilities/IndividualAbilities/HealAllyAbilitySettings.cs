using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealAllyAbilitySettings : MonoBehaviour, IAbilitySettings
{
    public GameObject healingPrefab; // Assign this in the Inspector
    public MechStats.AbilityType Type = MechStats.AbilityType.Heal;
    public AbilityRules.DamageType HealthEffect = AbilityRules.DamageType.Heals;
    public AbilityRules.ScalingType ScalingMethod = AbilityRules.ScalingType.Singular;
    public AbilityRules.TileTargetType TileTargetingMethod = AbilityRules.TileTargetType.Ally;
    public AbilityRules.PrefabSummoningPlacement PrefabPlacementMethod = AbilityRules.PrefabSummoningPlacement.None;
    public AbilityRules.MovementImpactType MovementEffect = AbilityRules.MovementImpactType.None;
    public bool RequiresLineOfSight = true;
    public AbilityRules.EntityHealthTargetType HealthTarget = AbilityRules.EntityHealthTargetType.Pilot;
    public Sound activationSFX;
    public bool doesThisAbilityAddToDefense = false;



    public IAbilityStrategy GiveAbility() {
        var traits = new AbilityTraits
        {
            Type = Type,
            HealthEffect = HealthEffect,
            ScalingMethod = ScalingMethod,
            TileTargetingMethod = TileTargetingMethod,
            PrefabPlacementMethod = PrefabPlacementMethod,
            MovementEffect = MovementEffect,
            RequiresLineOfSight = RequiresLineOfSight,
            HealthTarget = HealthTarget,
            PrefabToSummon = healingPrefab, // Use prefab from component
            addsToMechDefense = doesThisAbilityAddToDefense // adds to defense
        };

        return new GenericAbilityStrategy(traits);
    }

    public Sound GiveAbilitySound() {
        return activationSFX;
    }
}
