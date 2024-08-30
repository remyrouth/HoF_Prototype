using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// AbilityExecutionManager
public class AbilityExecutionManager : MonoBehaviour
{
    private Dictionary<MechStats.AbilityType, IAbilitySettings> _abilityFactories = new Dictionary<MechStats.AbilityType, IAbilitySettings>();

    void Awake()
    {
        // Register ability factories
        GameObject childObject = new GameObject("ChildObjectAbilityHolder");
        _abilityFactories[MechStats.AbilityType.Laser] = childObject.AddComponent<LaserAbilitySettings>();
        _abilityFactories[MechStats.AbilityType.RockSummon] = childObject.AddComponent<RockSummonAbilitySettings>();
        _abilityFactories[MechStats.AbilityType.TeleportToTile] = childObject.AddComponent<TeleportToTileAbilitySettings>();
        _abilityFactories[MechStats.AbilityType.Heal] = childObject.AddComponent<HealAllyAbilitySettings>();
    }

    public bool InputAbilityInformationSources(MechStats.AbilityMechSlot ability, PlayerController character, GameObject tileTarget)
    {
        if (_abilityFactories.TryGetValue(ability.GetAbilityType(), out IAbilitySettings settingsOutput))
        {
            IAbilityStrategy abilityStrategy = settingsOutput.GiveAbility();
             Debug.Log("Type WAS actually found: " + ability.GetAbilityType().ToString());
            return abilityStrategy.Execute(ability, character, tileTarget);
        } else {
            Debug.Log("Type was not found: " + ability.GetAbilityType().ToString());
        }

        return false;
    }
}
