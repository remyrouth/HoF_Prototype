using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAbilitySettings
{
    public IAbilityStrategy GiveAbility();

    public Sound GiveAbilitySound();
}
