using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Character", fileName = "New Character")]
public class Character : ScriptableObject
{
    public string Name;
    public Sprite Sprite;
    public float VoicePitch = 1f;
    public Trade[] PossibleTrades;
    public int StartReputation;
    public bool ExplodeIfDenied = false;
}
