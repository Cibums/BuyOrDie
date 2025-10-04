using System.Collections;
using UnityEngine;

public class UserInterfaceController : MonoBehaviour
{
    public Transform InventoryPanel;
    public Transform MessageBoxPanel;
    public Transform MessageBoxOptionsPanel;
    public GameObject MessageBoxOptionPrefab;
    public GameObject InventoryItemPrefab;

    public static UserInterfaceController Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void UpdateInventory()
    {
        foreach (Transform child in InventoryPanel)
        {
            Destroy(child.gameObject);
        }

        foreach (Item item in GameController.Instance.State.Inventory)
        {
            var itemUI = Instantiate(InventoryItemPrefab, InventoryPanel);
            itemUI.GetComponent<ItemBehaviour>().UpdateItem(item);
        }
    }

    public IEnumerator ShowTradeMessage(Trade trade, TradeAction action = TradeAction.None)
    {
        switch (action)
        {
            case TradeAction.Confirm:
                yield return ShowMessage(trade.ConfirmMessage);
                GameController.Instance.TriggerNextCustomer();
                break;
            case TradeAction.Deny:
                yield return ShowMessage(trade.DenyMessage);
                GameController.Instance.TriggerNextCustomer();
                break;
            default:
                yield return ShowMessage(trade.Message);

                foreach (TradeOption option in trade.Options)
                {
                    var optionUI = Instantiate(MessageBoxOptionPrefab, MessageBoxOptionsPanel);
                    optionUI.GetComponentInChildren<TMPro.TMP_Text>().SetText(option.Message);
                    optionUI.GetComponent<ChoiceButtonBehaviour>().Action = option.Action;
                }

                break;
        }
    }

    public IEnumerator ShowMessage(string message)
    {
        foreach (Transform child in MessageBoxOptionsPanel)
        {
            Destroy(child.gameObject);
        }

        var textComponent = MessageBoxPanel.GetComponentInChildren<TMPro.TMP_Text>();
        string text = "";

        MessageBoxPanel.gameObject.SetActive(true);
        textComponent.SetText("");

        foreach (char c in message)
        {
            text += c;
            textComponent.SetText(text);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
