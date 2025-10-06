using System;
using System.Collections.Generic;

[Serializable]
public class GameStateSaveData
{
    public List<ReputationEntry> CharacterReputations = new();
    public List<int> Inventory = new();
    public int Money = 100;
    public int CurrentRent = 20;
    public int TimeUntilRent = 120;
    public int CurrentCustomerId = -1;
    public int CurrentTradeOfferIndex;
    public bool InTrade = false;
    public GameSettings Settings = new();
}

[Serializable] public struct ReputationEntry { public int CharacterId; public float Reputation; }

[Serializable]
public class GameSettings
{
    public int MasterVolume = 30;
}
