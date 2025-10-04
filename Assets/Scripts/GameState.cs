using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameState
{
    public Dictionary<int, float> CharacterRepuations = new Dictionary<int, float>();
    public List<Item> Inventory;
    private int money = 100;
    public int Money
    {
        get => money;
        set
        {
            money = value;
            UserInterfaceController.Instance.UpdateMoneyDisplay();
            SoundController.Instance.PlaySoundEffect(SoundEffectType.CashRegister);
        }
    }
    public int CurrentRent = 20;
    public int TimeUntilRent = 120;
    private Character currentCustomer;
    public Character CurrentCustomer
    {
        get => currentCustomer;
        set
        {
            Debug.Log("New customer: " + value.Name);
            currentCustomer = value;
            UserInterfaceController.Instance.UpdateCharacter();
        }
    }
    public int CurrentTradeOfferIndex;

    public GameSettings Settings = new GameSettings();
}

public class GameSettings
{
    public int MasterVolume = 30;
}