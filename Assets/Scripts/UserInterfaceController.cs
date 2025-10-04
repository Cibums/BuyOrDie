using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class UserInterfaceController : MonoBehaviour
{
    public Transform InventoryPanel;
    public Transform MessageBoxPanel;
    public Transform MessageBoxOptionsPanel;
    public Transform CharacterTransform;
    public Transform MoneyText;
    public Transform RentTimerText;
    public bool characterIsWalking;
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

    public void UpdateMoneyDisplay()
    {
        var moneyText = MoneyText.GetComponent<TMPro.TMP_Text>();
        moneyText.SetText(GameController.Instance.State.Money.ToString() + ":-");
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

    public IEnumerator CharacterWalkIn()
    {
        SoundController.Instance.PlaySoundEffect(SoundEffectType.NewCustomer);
        characterIsWalking = true;
        CharacterTransform.gameObject.GetComponent<Animator>().SetTrigger("In");
        yield return new WaitUntil(() => CharacterTransform.localPosition.x <= 0.35f);
        characterIsWalking = false;
    }

    public IEnumerator CharacterWalkOut()
    {
        MessageBoxPanel.gameObject.SetActive(false);
        characterIsWalking = true;
        CharacterTransform.gameObject.GetComponent<Animator>().SetTrigger("Out");
        yield return new WaitUntil(() => CharacterTransform.localPosition.x >= 2.95f);
        characterIsWalking = false;
    }

    public IEnumerator ShowTradeMessage(Trade trade, TradeAction action = TradeAction.None)
    {
        yield return new WaitUntil(() => !characterIsWalking);

        switch (action)
        {
            case TradeAction.Confirm:
                yield return ShowMessage(trade.ConfirmMessage);
                yield return new WaitForSeconds(1f);
                yield return StartCoroutine(CharacterWalkOut());
                GameController.Instance.TriggerNextCustomer();
                break;
            case TradeAction.Deny:
                yield return ShowMessage(trade.DenyMessage);
                yield return new WaitForSeconds(1f);
                yield return StartCoroutine(CharacterWalkOut());
                GameController.Instance.TriggerNextCustomer();
                break;
            default:
                yield return ShowMessage(trade.Message);

                foreach (TradeOption option in trade.Options)
                {
                    var optionUI = Instantiate(MessageBoxOptionPrefab, MessageBoxOptionsPanel);
                    optionUI.GetComponentInChildren<TMPro.TMP_Text>().SetText(option.Message);
                    ChoiceButtonBehaviour choiceButton = optionUI.GetComponent<ChoiceButtonBehaviour>();
                    choiceButton.Action = option.Action;
                    choiceButton.IsForced = trade.IsForced;
                    choiceButton.CheckEligibility();
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
            yield return new WaitForSeconds(0.03f);
        }
    }

    internal void UpdateRentTimerText()
    {
        RentTimerText.GetComponent<TMPro.TMP_Text>().SetText($"${GameController.Instance.State.CurrentRent} rent in: {GameController.Instance.State.TimeUntilRent}s");
    }

    internal void UpdateCharacter()
    {
        Debug.Log("Updating character sprite");
        CharacterTransform.GetComponent<SpriteRenderer>().sprite = GameController.Instance.State.CurrentCustomer.Sprite;
    }
}
