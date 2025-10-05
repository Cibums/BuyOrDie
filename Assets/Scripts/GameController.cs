using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public Character[] AllCharacters;
    public Item[] AllItems;
    public GameState State = new();
    public int RentTimer = 120;

    public GameObject TooltipUIPrefab;

    private readonly int inventorySize = 5;

    public static GameController Instance;

    public static bool IsGameOver = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void SaveNow() => SaveSystem.Save(State);

    private void OnApplicationPause(bool paused)
    {
        if (paused) SaveNow();
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus) SaveNow();
    }

    public void RestartGame()
    {
        SaveSystem.Clear();
        State = new GameState();
        SaveNow();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
        IsGameOver = true;
        SaveSystem.Clear();
        UserInterfaceController.Instance.ShowLoseScreen(reason);
    }

    private void Start()
    {
        SaveSystem.TryLoad(State);
        Debug.Log("Current in trade: " + State.InTrade);
        TriggerNextCustomer();
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

    public void TriggerNextCustomer(HashSet<int> excludeIds = null)
    {
        Debug.Log("In Trade: " + State.InTrade);
        
        if (State.InTrade)
        {
            Trade currentTrade = State.CurrentCustomer.PossibleTrades[State.CurrentTradeOfferIndex];
            StartCoroutine(UserInterfaceController.Instance.CharacterWalkIn());
            StartCoroutine(UserInterfaceController.Instance.ShowTradeMessage(currentTrade));
            return;
        }

        State.InTrade = true;

        if (excludeIds == null)
        {
            excludeIds = new HashSet<int>();
        }

        if (excludeIds.Count >= AllCharacters.Length)
        {
            Debug.Log("No more characters available!");
            LoseGame("You have no customers left!");
            return;
        }

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
            .Where(t => ValidTrade(t, currentReputation))
            .ToArray();

        Debug.Log($"Found {validTrades.Length} valid trades for {randomCharacter.Name}");

        if (validTrades.Length > 0)
        {
            int randomTradeIndex = Random.Range(0, validTrades.Length);
            Trade randomTrade = validTrades[randomTradeIndex];

            State.CurrentCustomer = randomCharacter;
            State.CurrentTradeOfferIndex = randomCharacter.PossibleTrades.ToList().IndexOf(randomTrade);

            SaveNow();

            StartCoroutine(UserInterfaceController.Instance.CharacterWalkIn());
            Debug.Log("Offering trade: " + randomTrade.Item.Name + " (" + randomTrade.TradeType + ") for " + randomTrade.Price);
            StartCoroutine(UserInterfaceController.Instance.ShowTradeMessage(randomTrade));
        }
        else
        {
            excludeIds.Add(characterId);
            
            foreach (int id in excludeIds)
            {
                Debug.Log("Excluding character with name: " + AllCharacters[id].Name);
            }

            State.InTrade = false;
            TriggerNextCustomer(excludeIds); 
        }
    }

    private bool ValidTrade(Trade t, int currentReputation)
    {
        Debug.Log("Reputation needed: " + t.MinimumRequiredReputation + ", current: " + currentReputation);

        bool tradeable = t.MinimumRequiredReputation <= currentReputation && currentReputation <= t.MaximumRequiredReputation;
        bool buyable = t.TradeType == TradeType.Sell && !InventoryFull && !State.Inventory.Contains(t.Item);
        bool sellable = t.TradeType == TradeType.Buy && State.Inventory.Contains(t.Item);

        Debug.Log($"Trade {t.Item.Name} tradeable: {tradeable}, buyable: {buyable}, sellable: {sellable}, hasItem: {State.Inventory.Contains(t.Item)}, inventoryFull: {InventoryFull}, inventoryCount: {State.Inventory.Count}, tradeType: {t.TradeType}");

        return tradeable && (buyable || sellable);
    }

    public void Extinguish()
    {
        Trade currentTrade = State.CurrentCustomer.PossibleTrades[State.CurrentTradeOfferIndex];
        if (currentTrade.IsExtinguishable)
        {
            ConfirmCurrentTradeOffer();
        }
    }

    public void ConfirmCurrentTradeOffer()
    {
        State.InTrade = false;

        Trade currentTrade = State.CurrentCustomer.PossibleTrades[State.CurrentTradeOfferIndex];
        State.CharacterRepuations[State.CurrentCustomer.Id] += currentTrade.ReputationIncrease;

        Debug.Log("Current customer: " + State.CurrentCustomer.Name + " (ID: " + State.CurrentCustomer.Id + ")");
        Debug.Log("Current trade: " + currentTrade.Item.Name + " (Type: " + currentTrade.TradeType + ", Price: " + currentTrade.Price + ")");
        Debug.Log("Confirming trade: " + currentTrade.Item.Name + " (" + currentTrade.TradeType + ") for " + currentTrade.Price);

        if (currentTrade.TradeType == TradeType.Sell)
        {
            AddItem(currentTrade.Item);
            UserInterfaceController.Instance.UpdateInventory();
        }
        else if (currentTrade.TradeType == TradeType.Buy)
        {
            RemoveItem(currentTrade.Item);
            UserInterfaceController.Instance.UpdateInventory();
        }

        if (currentTrade.Price != 0) {
            State.Money += currentTrade.TradeType == TradeType.Buy ? currentTrade.Price : -currentTrade.Price;   
        }

        SaveNow();

        StartCoroutine(UserInterfaceController.Instance.ShowTradeMessage(currentTrade, TradeAction.Confirm));
    }

    public void DenyCurrentTradeOffer(bool force = false)
    {
        State.InTrade = false;

        if (force)
        {
            StopAllCoroutines();
            UserInterfaceController.Instance.MessageBoxPanel.gameObject.SetActive(false);
            UserInterfaceController.Instance.characterIsWalking = false;
            State.InTrade = false;
            UserInterfaceController.Instance.CharacterTransform.gameObject.SetActive(false);

            TriggerNextCustomer();
            return;
        }

        Trade currentTrade = State.CurrentCustomer.PossibleTrades[State.CurrentTradeOfferIndex];
        State.CharacterRepuations[State.CurrentCustomer.Id] -= currentTrade.ReputationIncrease;

        SaveNow();

        StartCoroutine(UserInterfaceController.Instance.ShowTradeMessage(currentTrade, TradeAction.Deny));
    }

    private bool InventoryFull => State.Inventory.Count >= inventorySize;
}
