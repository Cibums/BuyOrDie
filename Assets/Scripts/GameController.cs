using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public Character[] AllCharacters;
    public GameState State;
    public int RentTimer = 120;

    public GameObject TooltipUIPrefab;

    private readonly int inventorySize = 5;

    public static GameController Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= 1f)
        {
            if (State.TimeUntilRent > 0)
            {
                State.TimeUntilRent--;
                UserInterfaceController.Instance.UpdateRentTimerText();
            }
            else
            {
                State.Money -= State.CurrentRent;

                if (State.Money < 0)
                {
                    LoseGame("You couldn't pay the rent!");
                }

                State.TimeUntilRent = RentTimer;
            }
            timer = 0f;
        }
    }

    private void LoseGame(string reason)
    {
        Debug.Log("You lose! " + reason);
    }

    private void Start()
    {
        TriggerNextCustomer();
        UserInterfaceController.Instance.UpdateMoneyDisplay();
    }

    public void AddItem(Item item)
    {
        if (!InventoryFull)
        {
            State.Inventory.Add(item);
        }
    }

    public bool RemoveItem(Item item)
    {
        if (item == null) return false;
        return State.Inventory.Remove(item);
    }

    public void TriggerNextCustomer()
    {
        Character randomCharacter = AllCharacters[Random.Range(0, AllCharacters.Length)];
        int characterId = randomCharacter.Id;

        int currentReputation = AllCharacters[characterId].StartReputation;
        if (State.CharacterRepuations.ContainsKey(characterId))
        {
            currentReputation = (int)State.CharacterRepuations[characterId];
        }
        else
        {
            State.CharacterRepuations[characterId] = currentReputation;
        }

        Debug.Log($"Current reputation with {randomCharacter.Name} is {currentReputation}");

        var validTrades = randomCharacter.PossibleTrades
            .Where(t =>
                t.MinimumRequiredReputation >= currentReputation &&
                (t.TradeType != TradeType.Buy || (!InventoryFull && !State.Inventory.Contains(t.Item)))
            )
            .ToArray();

        Debug.Log($"Found {validTrades.Length} valid trades for {randomCharacter.Name}");

        if (validTrades.Length > 0)
        {
            int randomTradeIndex = Random.Range(0, validTrades.Length);
            Trade randomTrade = validTrades[randomTradeIndex];

            State.CurrentCustomer = randomCharacter;
            State.CurrentTradeOfferIndex = randomTradeIndex;

            StartCoroutine(UserInterfaceController.Instance.CharacterWalkIn());
            StartCoroutine(UserInterfaceController.Instance.ShowTradeMessage(randomTrade));
        }
        else
        {
            throw new System.IndexOutOfRangeException("No valid trades available for this character.");   
        }
    }

    public void ConfirmCurrentTradeOffer()
    {
        Trade currentTrade = State.CurrentCustomer.PossibleTrades[State.CurrentTradeOfferIndex];
        State.CharacterRepuations[State.CurrentCustomer.Id] += currentTrade.ReputationIncrease;
        
        if(currentTrade.TradeType == TradeType.Sell)
        {
            AddItem(currentTrade.Item);
            UserInterfaceController.Instance.UpdateInventory();
        }
        else if (currentTrade.TradeType == TradeType.Buy)
        {
            RemoveItem(currentTrade.Item);
            UserInterfaceController.Instance.UpdateInventory();
        }

        State.Money += currentTrade.TradeType == TradeType.Buy ? currentTrade.Price : -currentTrade.Price;

        StartCoroutine(UserInterfaceController.Instance.ShowTradeMessage(currentTrade, TradeAction.Confirm));
    }

    public void DenyCurrentTradeOffer()
    {
        Trade currentTrade = State.CurrentCustomer.PossibleTrades[State.CurrentTradeOfferIndex];
        State.CharacterRepuations[State.CurrentCustomer.Id] -= currentTrade.ReputationIncrease;

        StartCoroutine(UserInterfaceController.Instance.ShowTradeMessage(currentTrade, TradeAction.Deny));
    }

    private bool InventoryFull => State.Inventory.Count >= inventorySize;
}
