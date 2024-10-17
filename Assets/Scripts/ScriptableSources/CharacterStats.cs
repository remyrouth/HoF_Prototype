using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterStats", menuName = "Character/NewPilot")]
public class CharacterStats : ScriptableObject
{
    [SerializeField]
    private string pilotName; 
    [SerializeField]
    private int laserStrength;
    [SerializeField]
    private int laserRange;
    [SerializeField]
    private int ballisticStrength;
    [SerializeField]
    private int ballisticRange;
    [SerializeField]
    private int speed;
    [SerializeField]
    private int health;
    [SerializeField]
    private int currentExperiencePoint = 0;
    [SerializeField]
    private Sprite characterSprite;

    [SerializeField]
    private int clarityGainedFromMovements = 1;
    [SerializeField]
    private int clarityGainedFromAttacks = 1;

    private enum Faction {
        PrincipalityOfAmerica, 
        USA, 
        // NeoDahomey, Alter name to new African Union Faction Title
        SolInvicti, 
        Evolites, 
        Morningstars // etc. as decided by story
    }

    [SerializeField]
    private Faction pilotFaction;

    public string GetPilotName() {
        return pilotName;
    }

    public int GetPilotHealth()
    {
        return health;
    }

    public int GetLaserStrength()
    {
        return laserStrength;
    }

    public int GetLaserRange()
    {
        return laserRange;
    }

    public int GetBallisticStrength()
    {
        return ballisticStrength;
    }

    public int GetBallisticRange()
    {
        return ballisticRange;
    }

    public int GetPilotSpeed()
    {
        return speed;
    }

    public int GetCurrentExperiencePoint()
    {
        return currentExperiencePoint;
    }

    public Sprite GetCharacterSprite()
    {
        return characterSprite;
    }

    public int GetMoveClarity() {
        return clarityGainedFromMovements;
    }

    public int GetAttackClarity() {
        return clarityGainedFromAttacks;
    }

    public string GetPilotFaction()
    {
        return formatFaction(pilotFaction);
    }
    
    // Placeholder for changing factions (defect)
    public void swapPilotFaction(string faction)
    {
        // Change faction based on input. Set to Bell's faction?
        // EX: pilotFaction = Faction.PrincipalityOfAmerica;
        Debug.Log("swapPilotFaction in CharacterStats not implemented.");
    }

    private string formatFaction(Faction unfor)
    {
        switch (unfor)
        {
            case Faction.USA:
                return "United States of America";
            case Faction.PrincipalityOfAmerica:
                return "Principality of America";
            case Faction.SolInvicti:
                return "Sol-Invicti";
            default:
                return unfor.ToString();
        }
    }
}
