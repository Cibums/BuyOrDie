using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Item", fileName = "New Item")]
public class Item : ScriptableObject
{
    public string Name;
    public Sprite Sprite;

    public int Id => GameController.Instance.AllItems.ToList().IndexOf(this);
}

[CreateAssetMenu(menuName = "Action Item", fileName = "New Action Item")]
public class ActionItem : Item
{
    public ItemTrigger Trigger;
    public bool Reusable = false;
    public SoundEffectType SoundEffect;
    public ItemAction[] Actions;
    public float ActionValue = 0f;
    public string Description;
}

public enum ItemTrigger
{
    OnClick,
    OnSell,
    EverySecond
}

public enum ItemAction
{
    IncreaseMoney,
    DecreaseMoney,
    IncreaseItemValue,
    DecreaseItemValue,
    IncreaseReputation,
    DecreaseReputation,
    Extinguish,
    DenyTrade,
    RemoveThisItem
}