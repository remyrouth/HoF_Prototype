using UnityEngine;
using UnityEngine.SocialPlatforms;

[CreateAssetMenu(fileName = "NewCharacterStats", menuName = "Character/NewPilot")]
public class CharacterStats : ScriptableObject
{
    [SerializeField] private string pilotName; 
    [SerializeField] private int laserStrength;
    [SerializeField] private int laserRange;
    [SerializeField] private int ballisticStrength;
    [SerializeField] private int ballisticRange;
    [SerializeField] private int speed;
    [Range(0f, 1f)] [SerializeField] private float dodgeChance;
    [Range(0, 100)] [SerializeField] private int hailResistance;
    [SerializeField] private int health;
    [SerializeField] private int currentExperiencePoint = 0;
    [SerializeField] private Sprite characterSprite;
    

    [SerializeField] private int clarityGainedFromMovements = 1;
    [SerializeField] private int clarityGainedFromAttacks = 1;


    public string GetPilotName() {
        return pilotName;
    }

    public float GetDodgeChance() {
        return dodgeChance;
    }

    public int GetHailResistance()
    {
        return hailResistance;
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
}
