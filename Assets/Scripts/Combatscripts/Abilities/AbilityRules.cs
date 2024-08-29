using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AbilityRules
{
    public enum DamageType
    {
        None,
        Damages,
        Heals
    }

    public enum ScalingType
    {
        Singular,
        FartherMeansGreaterEffect,
        CloserMeansGreaterEffect
    }

    public enum TileTargetType
    {
        Empty,
        Ally,
        Enemy
    }

    public enum PrefabSummoningPlacement
    {
        None,
        AtTarget,
        AtSource,
        AtBothSourceAndTarget
    }

    public enum MovementImpactType
    {
        None,
        PushAwayFromTarget,
        PullTowardsTarget,
        TeleportToTile
    }

    public enum EntityHealthTargetType
    {
        None,
        Pilot,
        Mech,
        Both
    }
}