using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewTeamBuilder", menuName = "Character/TeamBuilder")]
public class TeamBuilder : ScriptableObject
{
    public List<MechStats> mechs;
    public List<CharacterStats> pilots;

    public void AddMech(MechStats mech)
    {
        if (!mechs.Contains(mech))
        {
            mechs.Add(mech);
        }
    }

    public void AddPilot(CharacterStats pilot)
    {
        if (!pilots.Contains(pilot))
        {
            pilots.Add(pilot);
        }
    }

    public void RemoveMech(MechStats mech)
    {
        if (mechs.Contains(mech))
        {
            mechs.Remove(mech);
        }
    }

    public void RemovePilot(CharacterStats pilot)
    {
        if (pilots.Contains(pilot))
        {
            pilots.Remove(pilot);
        }
    }

    public MechStats GetMech(int index)
    {
        if (index >= 0 && index < mechs.Count)
        {
            return mechs[index];
        }
        return null;
    }

    public CharacterStats GetPilot(int index)
    {
        if (index >= 0 && index < pilots.Count)
        {
            return pilots[index];
        }
        return null;
    }

    public int PilotLength() {
        return pilots.Count;
    }

    public int MechLength() {
        return mechs.Count;
    }
}
