using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterStats", menuName = "Character/Stats")]
public class CharacterStats : ScriptableObject
{
    public int strength;
    public int speed;
    public int health;
    public Sprite characterSprite;
}
