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
        if (Item != null)
        {
            TooltipText = Item.Name;
        }

        if (Item.GetType() == typeof(ActionItem))
        {
            TooltipText += ": " + ItemTriggerDisplayNames[((ActionItem)Item).Trigger] + " - " + ((ActionItem)Item).Description;
        }
    }

    public void UpdateItem(Item item)
    {
        Item = item;
        SetTooltipText();
        UpdateItem();
    }

    public void UpdateItem()
    {
        GetComponent<Image>().sprite = Item.Sprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Item.GetType() != typeof(ActionItem)) {
            return;
        }
        
        ActionItem actionItem = (ActionItem)Item;
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
                    GameController.Instance.State.CharacterRepuations[GameController.Instance.State.CurrentCustomer.Id] += Mathf.RoundToInt(actionItem.ActionValue);
                    break;
                case ItemAction.DecreaseReputation:
                    GameController.Instance.State.CharacterRepuations[GameController.Instance.State.CurrentCustomer.Id] -= Mathf.RoundToInt(actionItem.ActionValue);
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
