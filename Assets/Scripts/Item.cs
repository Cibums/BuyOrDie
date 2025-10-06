using UnityEngine;
using UnityEngine.Scripting;

[Preserve]
[CreateAssetMenu(menuName = "Item", fileName = "New Item")]
public class Item : ScriptableObject
{
    public string Name;
    public Sprite Sprite;
}