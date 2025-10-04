using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameState
{
    public Dictionary<int, float> CharacterRepuations;
    public List<Item> Inventory;
    public int Money;
}
