using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Character", fileName = "New Character")]
public class Character : ScriptableObject
{
    public string Name;
    public Sprite Sprite;
    public VoiceType VoiceType;
    public Trade[] PossibleTrades;
    public int StartReputation;
    public int Id => GameController.Instance.AllCharacters.ToList().IndexOf(this);
}

public enum VoiceType {
    Male,
    Female,
    Alien
}
