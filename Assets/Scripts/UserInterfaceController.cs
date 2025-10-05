using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class UserInterfaceController : MonoBehaviour
{
    [InspectorLabel("Inventory")]
    public Transform InventoryPanel;

    [InspectorLabel("Message Box")]
    public Transform MessageBoxPanel;
    public Transform MessageBoxText;
    public Transform MessageBoxNameText;
    public Transform MessageBoxOptionsPanel;

    [InspectorLabel("Lose Screen")]
    public Transform LoseScreenPanel;
    public TMPro.TMP_Text LoseScreenText;
    public TMPro.TMP_Text LoseScreenReasonText;

    [InspectorLabel("Character")]
    public Transform CharacterTransform;

    [InspectorLabel("Status")]
    public Transform MoneyText;
    public Transform RentTimerText;

    [InspectorLabel("Prefabs")]
    public GameObject MessageBoxOptionPrefab;
    public GameObject InventoryItemPrefab;

    [HideInInspector]
    public bool characterIsWalking;

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
        CharacterTransform.gameObject.SetActive(true);
        CharacterTransform.gameObject.GetComponent<Animator>().SetTrigger("In");
        yield return new WaitForSeconds(0.5f);
        Debug.Log($"CharacterWalkIn: localPosition.x = {CharacterTransform.localPosition.x}");
        yield return new WaitUntil(() => CharacterTransform.localPosition.x <= 0.2f);
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

    public void ShowLoseScreen(string reason)
    {
        LoseScreenPanel.gameObject.SetActive(true);
        StartCoroutine(WriteText(LoseScreenText, "You Lose!", () => SoundController.Instance.PlaySoundEffect(SoundEffectType.Type)));
        StartCoroutine(WriteText(LoseScreenReasonText, reason, () => SoundController.Instance.PlaySoundEffect(SoundEffectType.Type)));
    }

    public IEnumerator ShowMessage(string message)
    {
        foreach (Transform child in MessageBoxOptionsPanel)
        {
            Destroy(child.gameObject);
        }

        MessageBoxNameText.GetComponent<TMPro.TMP_Text>().SetText(GameController.Instance.State.CurrentCustomer.Name);

        var textComponent = MessageBoxText.GetComponent<TMPro.TMP_Text>();

        MessageBoxPanel.gameObject.SetActive(true);
        textComponent.SetText("");

        yield return WriteText(textComponent, message, () => SoundController.Instance.PlayTalkSound());
    }

    IEnumerator WriteText(TMPro.TMP_Text textComponent, string message, Action forEachCharacter = null)
    {
        string text = "";
        foreach (char c in message)
        {
            forEachCharacter?.Invoke();

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
        CharacterTransform.GetComponent<SpriteRenderer>().sprite = GameController.Instance?.State?.CurrentCustomer?.Sprite;
    }
}
