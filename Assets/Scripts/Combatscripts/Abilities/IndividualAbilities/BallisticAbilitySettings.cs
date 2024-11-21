using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallisticAbilitySettings : MonoBehaviour, IAbilitySettings
{
    public GameObject attackPrefab; // Assign this in the Inspector
    public MechStats.AbilityType Type = MechStats.AbilityType.Ballistic;
    public AbilityRules.DamageType HealthEffect = AbilityRules.DamageType.Damages;
    public AbilityRules.ScalingType ScalingMethod = AbilityRules.ScalingType.Singular;
    public AbilityRules.TileTargetType TileTargetingMethod = AbilityRules.TileTargetType.Enemy;
    public AbilityRules.PrefabSummoningPlacement PrefabPlacementMethod = AbilityRules.PrefabSummoningPlacement.None;
    public AbilityRules.MovementImpactType MovementEffect = AbilityRules.MovementImpactType.None;
    public bool RequiresLineOfSight = true;
    public AbilityRules.EntityHealthTargetType HealthTarget = AbilityRules.EntityHealthTargetType.Mech;
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
            PrefabToSummon = attackPrefab, // Use prefab from component
            addsToMechDefense = doesThisAbilityAddToDefense // adds to defense
        };

        return new GenericAbilityStrategy(traits);
    }

    public Sound GiveAbilitySound() {
        return activationSFX;
    }
}
