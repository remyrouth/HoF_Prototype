using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterStats", menuName = "Character/NewPilot")]
public class CharacterStats : ScriptableObject
{
    public int strength;
    public int attackRange;
    public int speed;
    public int health;
    public int currentExperiencePoint = 0;
    public Sprite characterSprite;



    public int clarityGainedFromMovements = 1;
    public int clarityGainedFromAttacks = 1;
}
