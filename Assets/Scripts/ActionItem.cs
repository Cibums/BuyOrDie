using UnityEngine;
using UnityEngine.Scripting;

[Preserve]
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
    RemoveThisItem,
    DestroyUniverse
}