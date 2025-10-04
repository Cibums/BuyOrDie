using System;
using UnityEngine;

[Serializable]
public class Trade
{
    public Item Item;
    public TradeType TradeType;
    public int MinimumRequiredReputation = 0;
    public int Price = 0;
    public TradeOption[] Options;

    [SerializeField, TextArea]
    private string message;

    public string Message
    {
        get
        {
            if (string.IsNullOrEmpty(message))
                return string.Empty;

            string processed = message;
            processed = processed.Replace("{item}", Item.Name); 
            processed = processed.Replace("{price}", Price.ToString());
            processed = processed.Replace("{type}", TradeType.ToString());

            return processed;
        }
        set
        {
            message = value;
        }
    }
}

public enum TradeType
{
    Buy,
    Sell
}

[Serializable]
public class TradeOption
{
    public string Message;
    public Action Action;
}