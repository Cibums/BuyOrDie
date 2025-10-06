using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemBehaviour : TooltipBehaviour, IPointerClickHandler
{
    public Item Item;

    void Awake()
    {
        SetTooltipText();
    }

    private static readonly Dictionary<ItemTrigger, string> ItemTriggerDisplayNames = new()
    {
        { ItemTrigger.OnClick, "On Click" },
        { ItemTrigger.OnSell, "On Sell" },
        { ItemTrigger.EverySecond, "Every Second" }
    };

    private void SetTooltipText()
    {
        TooltipText = Item ? Item.Name : string.Empty;
        if (Item is ActionItem ai)
            TooltipText += $": {ItemTriggerDisplayNames[ai.Trigger]} - {ai.Description}";
    }

    public void UpdateItem(Item item)
    {
        Item = item;
        SetTooltipText();
        UpdateItem();
    }

    public void UpdateItem()
    {
        if (Item == null) return;
        var img = GetComponent<Image>();
        if (img) img.sprite = Item.Sprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var actionItem = Item as ActionItem;
        if (actionItem == null) return;
        Debug.Log("Playing sound effect: " + actionItem.SoundEffect);
        SoundController.Instance.PlaySoundEffect(actionItem.SoundEffect);

        foreach (var action in actionItem.Actions)
        {
            switch (action)
            {
                case ItemAction.IncreaseMoney:
                    GameController.Instance.State.Money += Mathf.RoundToInt(actionItem.ActionValue);
                    break;
                case ItemAction.DecreaseMoney:
                    GameController.Instance.State.Money -= Mathf.RoundToInt(actionItem.ActionValue);
                    break;
                case ItemAction.IncreaseReputation:
                    GameController.Instance.State.CharacterRepuations[GameMapper.CharacterToId(GameController.Instance.State.CurrentCustomer)] += Mathf.RoundToInt(actionItem.ActionValue);
                    break;
                case ItemAction.DecreaseReputation:
                    GameController.Instance.State.CharacterRepuations[GameMapper.CharacterToId(GameController.Instance.State.CurrentCustomer)] -= Mathf.RoundToInt(actionItem.ActionValue);
                    break;
                case ItemAction.IncreaseItemValue:
                    // Not implemented
                    break;
                case ItemAction.DecreaseItemValue:
                    // Not implemented
                    break;
                case ItemAction.DenyTrade:
                    GameController.Instance.DenyCurrentTradeOffer(true);
                    break;
                case ItemAction.Extinguish:
                    GameController.Instance.Extinguish();
                    break;
                case ItemAction.RemoveThisItem:
                    GameController.Instance.RemoveItem(actionItem);
                    UserInterfaceController.Instance.UpdateInventory();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
