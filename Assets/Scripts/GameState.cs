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
            if (suppressSideEffects) return;
            UserInterfaceController.Instance.UpdateMoneyDisplay();
            SoundController.Instance.PlaySoundEffect(SoundEffectType.CashRegister);
        }
    }
    public int CurrentRent = 20;
    public int TimeUntilRent = 120;
    private Character currentCustomer = null;
    public bool InTrade = false;
    public Character CurrentCustomer
    {
        get => currentCustomer;
        set
        {
            Debug.Log("New customer: " + value.Name);
            currentCustomer = value;
            if (suppressSideEffects) return;
            UserInterfaceController.Instance.UpdateCharacter();
        }
    }
    public int CurrentTradeOfferIndex;

    public GameSettings Settings = new GameSettings();

    private bool suppressSideEffects;

    public GameStateSaveData ToSaveData()
    {
        var d = new GameStateSaveData
        {
            Money = money,
            CurrentRent = CurrentRent,
            TimeUntilRent = TimeUntilRent,
            CurrentTradeOfferIndex = CurrentTradeOfferIndex,
            Settings = Settings,
            InTrade = InTrade,
            CurrentCustomerId = (currentCustomer != null) ? GameMapper.CharacterToId(currentCustomer) : -1,
        };

        d.CharacterReputations.Clear();
        foreach (var kv in CharacterRepuations)
            d.CharacterReputations.Add(new ReputationEntry { CharacterId = kv.Key, Reputation = kv.Value });

        d.Inventory.Clear();
        foreach (var item in Inventory)
            d.Inventory.Add(GameMapper.ItemToId(item));

        return d;
    }

    public void LoadFromSaveData(GameStateSaveData d)
    {
        suppressSideEffects = true;

        Money = d.Money;
        CurrentRent = d.CurrentRent;
        TimeUntilRent = d.TimeUntilRent;
        CurrentTradeOfferIndex = d.CurrentTradeOfferIndex;
        InTrade = d.InTrade;
        Settings = d.Settings ?? new GameSettings();

        CharacterRepuations.Clear();
        foreach (var rep in d.CharacterReputations)
            CharacterRepuations[rep.CharacterId] = rep.Reputation;

        Inventory.Clear();
        foreach (var entry in d.Inventory)
        {
            var item = GameMapper.IdToItem(entry);
            if (item != null) Inventory.Add(item);
        }

        CurrentCustomer = (d.CurrentCustomerId >= 0) ? GameMapper.IdToCharacter(d.CurrentCustomerId) : null;

        suppressSideEffects = false;

        UserInterfaceController.Instance.UpdateMoneyDisplay();
        UserInterfaceController.Instance.UpdateCharacter();
        UserInterfaceController.Instance.UpdateInventory();
        UserInterfaceController.Instance.UpdateRentTimerText();
    }
}