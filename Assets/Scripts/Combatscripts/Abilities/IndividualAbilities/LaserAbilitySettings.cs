using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserAbilitySettings : MonoBehaviour, IAbilitySettings
{
    public GameObject laserPrefab; // Assign this in the Inspector
    public MechStats.AbilityType Type = MechStats.AbilityType.Laser;
    public AbilityRules.DamageType HealthEffect = AbilityRules.DamageType.Damages;
    public AbilityRules.ScalingType ScalingMethod = AbilityRules.ScalingType.Singular;
    public AbilityRules.TileTargetType TileTargetingMethod = AbilityRules.TileTargetType.Enemy;
    public AbilityRules.PrefabSummoningPlacement PrefabPlacementMethod = AbilityRules.PrefabSummoningPlacement.None;
    public AbilityRules.MovementImpactType MovementEffect = AbilityRules.MovementImpactType.None;
    public bool RequiresLineOfSight = true;
    public AbilityRules.EntityHealthTargetType HealthTarget = AbilityRules.EntityHealthTargetType.Pilot;



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
            PrefabToSummon = laserPrefab // Use prefab from component
        };

        return new GenericAbilityStrategy(traits);
    }
}
