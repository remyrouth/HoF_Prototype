using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterStats", menuName = "Character/NewPilot")]
public class CharacterStats : ScriptableObject
{
    public int strength;
    public int attackRange;
    public int speed;
    public int health;
    public Sprite characterSprite;
}
