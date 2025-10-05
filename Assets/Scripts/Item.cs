using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Item", fileName = "New Item")]
public class Item : ScriptableObject
{
    public string Name;
    public Sprite Sprite;

    public int Id => GameController.Instance.AllItems.ToList().IndexOf(this);
}
