using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameState
{
    public Dictionary<int, float> CharacterRepuations = new Dictionary<int, float>();
    public List<Item> Inventory;
    public int Money;
    public Character CurrentCustomer;
    public int CurrentTradeOfferIndex;
}
