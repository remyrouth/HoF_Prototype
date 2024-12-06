using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HailAbilitySettings : MonoBehaviour, IAbilitySettings
{
    public GameObject HailPrefab; // Assign this in the Inspector
    public MechStats.AbilityType AbilityType = MechStats.AbilityType.ConvertEnemy;
    public AbilityRules.DamageType HealthEffect = AbilityRules.DamageType.None;
    public AbilityRules.ScalingType ScalingMethod = AbilityRules.ScalingType.Singular;
    public AbilityRules.TileTargetType TileTargetingMethod = AbilityRules.TileTargetType.Enemy;
    public AbilityRules.PrefabSummoningPlacement PrefabPlacementMethod = AbilityRules.PrefabSummoningPlacement.None;
    public AbilityRules.MovementImpactType MovementEffect = AbilityRules.MovementImpactType.None;
    public bool RequiresLineOfSight = true;
    
    // not sure if this should be None or Pilot
    public AbilityRules.EntityHealthTargetType HealthTarget = AbilityRules.EntityHealthTargetType.Pilot;
    public Sound HailSFX;
    public bool doesThisAbilityAddToDefense = false;
    

    public IAbilityStrategy GiveAbility()
    {
        var traits = new AbilityTraits
        {
            Type = AbilityType,
            HealthEffect = HealthEffect,
            ScalingMethod = ScalingMethod,
            TileTargetingMethod = TileTargetingMethod,
            PrefabPlacementMethod = PrefabPlacementMethod,
            MovementEffect = MovementEffect,
            RequiresLineOfSight = RequiresLineOfSight,
            HealthTarget = HealthTarget,
            PrefabToSummon = HailPrefab, // Use prefab from component
            addsToMechDefense = doesThisAbilityAddToDefense // adds to defense
        };

        return new GenericAbilityStrategy(traits);
    }

    public Sound GiveAbilitySound()
    {
        return HailSFX;
    }
}
