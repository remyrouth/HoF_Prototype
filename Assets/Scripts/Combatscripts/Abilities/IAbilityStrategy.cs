using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Interface for ability execution
public interface IAbilityStrategy
{
    bool Execute(MechStats.AbilityMechSlot ability, PlayerController character, GameObject tileTarget);
}