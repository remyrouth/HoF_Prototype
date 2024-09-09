using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// AbilityExecutionManager
public class AbilityExecutionManager : MonoBehaviour
{
    private Dictionary<MechStats.AbilityType, IAbilitySettings> _abilityFactories = new Dictionary<MechStats.AbilityType, IAbilitySettings>();
    SoundManager soundManager;

    void Awake()
    {
        // Register ability factories
        GameObject childObject = new GameObject("ChildObjectAbilityHolder");
        _abilityFactories[MechStats.AbilityType.Laser] = childObject.AddComponent<LaserAbilitySettings>();
        _abilityFactories[MechStats.AbilityType.RockSummon] = childObject.AddComponent<RockSummonAbilitySettings>();
        _abilityFactories[MechStats.AbilityType.TeleportToTile] = childObject.AddComponent<TeleportToTileAbilitySettings>();
        _abilityFactories[MechStats.AbilityType.Heal] = childObject.AddComponent<HealAllyAbilitySettings>();
        _abilityFactories[MechStats.AbilityType.Ballistic] = childObject.AddComponent<BallisticAbilitySettings>();
        _abilityFactories[MechStats.AbilityType.Combo] = childObject.AddComponent<ComboAbilitySettings>();
    }

    private void UseSoundEffect(IAbilitySettings settingsOutput) {
        if (soundManager == null) {
            soundManager = FindObjectOfType<SoundManager>();
        }
        if (soundManager == null) {
            soundManager = gameObject.AddComponent<SoundManager>();
        }

        SingleSoundPlayer soundPlayer = soundManager.GetOrCreateSoundPlayer(settingsOutput.GiveAbilitySound());
        soundPlayer.PlayFromForeignTrigger();
    }

    public bool InputAbilityInformationSources(MechStats.AbilityMechSlot ability, PlayerController character, GameObject tileTarget)
    {
        if (_abilityFactories.TryGetValue(ability.GetAbilityType(), out IAbilitySettings settingsOutput))
        {
            IAbilityStrategy abilityStrategy = settingsOutput.GiveAbility();
            // Debug.Log("Type WAS actually found: " + ability.GetAbilityType().ToString());
            bool abilitySucessfullyUsed = abilityStrategy.Execute(ability, character, tileTarget);
            if (abilitySucessfullyUsed) {
                UseSoundEffect(settingsOutput);
            }

            return abilitySucessfullyUsed;
        } else {
            Debug.Log("Type was not found: " + ability.GetAbilityType().ToString());
        }

        return false;
    }
}
