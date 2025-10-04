using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public Character[] AllCharacters;
    public GameState State;

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

    private void Start()
    {
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

    public void TriggerNextCustomer()
    {
        StartCoroutine(UserInterfaceController.Instance.ShowMessage(AllCharacters[Random.Range(0, AllCharacters.Length)].PossibleTrades[0]));
    }

    private bool InventoryFull => State.Inventory.Count >= inventorySize;
}
