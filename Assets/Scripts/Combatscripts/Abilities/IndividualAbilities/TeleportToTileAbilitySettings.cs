using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportToTileAbilitySettings : MonoBehaviour, IAbilitySettings
{
    public GameObject teleportPrefab; // Assign this in the Inspector
    public MechStats.AbilityType Type = MechStats.AbilityType.TeleportToTile;
    public AbilityRules.DamageType HealthEffect = AbilityRules.DamageType.None;
    public AbilityRules.ScalingType ScalingMethod = AbilityRules.ScalingType.Singular;
    public AbilityRules.TileTargetType TileTargetingMethod = AbilityRules.TileTargetType.Empty;
    public AbilityRules.PrefabSummoningPlacement PrefabPlacementMethod = AbilityRules.PrefabSummoningPlacement.None;
    public AbilityRules.MovementImpactType MovementEffect = AbilityRules.MovementImpactType.TeleportToTile;
    public bool RequiresLineOfSight = false;
    public AbilityRules.EntityHealthTargetType HealthTarget = AbilityRules.EntityHealthTargetType.None;
    public Sound activationSFX;



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
            PrefabToSummon = teleportPrefab // Use prefab from component
        };

        return new GenericAbilityStrategy(traits);
    }

    public Sound GiveAbilitySound() {
        return activationSFX;
    }
}
