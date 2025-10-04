using System;
using UnityEditor.Hardware;
using UnityEngine;

[Serializable]
public class Trade
{
    public Item Item;
    public TradeType TradeType;
    public int ReputationIncrease = 0;
    public int MinimumRequiredReputation = 0;
    public int Price = 0;
    public bool IsForced = false;
    public TradeOption[] Options;

    [SerializeField, TextArea]
    private string message;

    [SerializeField, TextArea]
    private string confirmMessage;

    [SerializeField, TextArea]
    private string denyMessage;

    public string Message
    {
        get
        {
            return ProcessMessage(message);
        }
        set
        {
            message = value;
        }
    }

    public string DenyMessage
    {
        get
        {
            return ProcessMessage(denyMessage);
        }
        set
        {
            denyMessage = value;
        }
    }

    public string ConfirmMessage
    {
        get
        {
            return ProcessMessage(confirmMessage);
        }
        set
        {
            confirmMessage = value;
        }
    }

    string ProcessMessage(string message)
    {
        if (string.IsNullOrEmpty(message))
            return string.Empty;

        string processed = message;
        processed = processed.Replace("{item}", Item.Name);
        processed = processed.Replace("{price}", Price.ToString());
        processed = processed.Replace("{type}", TradeType.ToString());

        return processed;
    }
}

public enum TradeType
{
    Buy,
    Sell
}

public enum TradeAction
{
    Confirm,
    Deny,
    None
}

[Serializable]
public class TradeOption
{
    public string Message;
    public TradeAction Action;
}