using UnityEngine;

public class ItemBehaviour : TooltipBehaviour
{
    public Item Item;

    void Awake()
    {
        SetTooltipText();
    }

    private void SetTooltipText()
    {
        if (Item != null)
        {
            TooltipText = Item.Name;
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
        
    }
}
