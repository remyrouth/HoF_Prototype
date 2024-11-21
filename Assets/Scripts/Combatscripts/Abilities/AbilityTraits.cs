using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ability traits
public class AbilityTraits
{
    public MechStats.AbilityType Type { get; set; }
    public AbilityRules.DamageType HealthEffect { get; set; }
    public AbilityRules.ScalingType ScalingMethod { get; set; }
    public AbilityRules.TileTargetType TileTargetingMethod { get; set; }
    public AbilityRules.PrefabSummoningPlacement PrefabPlacementMethod { get; set; }
    public AbilityRules.MovementImpactType MovementEffect { get; set; }
    public bool RequiresLineOfSight { get; set; }
    public AbilityRules.EntityHealthTargetType HealthTarget { get; set; }
    public GameObject PrefabToSummon { get; set; }
    public bool addsToMechDefense = false;
}