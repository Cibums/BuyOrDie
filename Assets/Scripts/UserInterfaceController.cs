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

    public IEnumerator ShowMessage(Trade trade)
    {
        MessageBoxPanel.gameObject.SetActive(true);

        var textComponent = MessageBoxPanel.GetComponentInChildren<TMPro.TMP_Text>();
        string text = "";

        foreach (char c in trade.Message)
        {
            text += c;
            textComponent.SetText(text);
            yield return new WaitForSeconds(0.1f);
        }

        foreach (TradeOption option in trade.Options)
        {
            var optionUI = Instantiate(MessageBoxOptionPrefab, MessageBoxOptionsPanel);
            optionUI.GetComponentInChildren<TMPro.TMP_Text>().SetText(option.Message);
        }
    }
}
