using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockSummonAbilitySettings : MonoBehaviour, IAbilitySettings
{
    public GameObject rockPrefab; // Assign this in the Inspector
    public MechStats.AbilityType Type = MechStats.AbilityType.RockSummon;
    public AbilityRules.DamageType HealthEffect = AbilityRules.DamageType.None;
    public AbilityRules.ScalingType ScalingMethod = AbilityRules.ScalingType.Singular;
    public AbilityRules.TileTargetType TileTargetingMethod = AbilityRules.TileTargetType.Empty;
    public AbilityRules.PrefabSummoningPlacement PrefabPlacementMethod = AbilityRules.PrefabSummoningPlacement.AtTarget;
    public AbilityRules.MovementImpactType MovementEffect = AbilityRules.MovementImpactType.None;
    public bool RequiresLineOfSight = false;
    public AbilityRules.EntityHealthTargetType HealthTarget = AbilityRules.EntityHealthTargetType.None;



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
            PrefabToSummon = rockPrefab // Use prefab from component
        };

        return new GenericAbilityStrategy(traits);
    }
}
