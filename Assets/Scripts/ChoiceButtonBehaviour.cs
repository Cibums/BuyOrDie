using UnityEngine;

public class ChoiceButtonBehaviour : MonoBehaviour
{
    public TradeAction Action;
    public bool IsForced = false;

    public void OnClick()
    {
        if (Action == TradeAction.Confirm)
        {
            GameController.Instance.ConfirmCurrentTradeOffer();
        }
        else if (Action == TradeAction.Deny)
        {
            GameController.Instance.DenyCurrentTradeOffer();
        }
    }

    public void CheckEligibility()
    {
        Trade currentTrade = GameController.Instance.State.CurrentCustomer?.PossibleTrades[GameController.Instance.State.CurrentTradeOfferIndex];
        if (currentTrade != null)
        {
            if (Action == TradeAction.Confirm)
            {
                bool canAfford = currentTrade.TradeType == TradeType.Sell ? GameController.Instance.State.Money >= currentTrade.Price : true;

                if (!canAfford)
                {
                    GetComponent<UnityEngine.UI.Button>().interactable = canAfford;
                    var textComponent = GetComponentInChildren<TMPro.TMP_Text>();
                    string text = textComponent.text;
                    textComponent.SetText(text + " - low funds");
                }
                else
                {
                    GetComponent<UnityEngine.UI.Button>().interactable = true;
                }
            }
            else if (!IsForced)
            {
                GetComponent<UnityEngine.UI.Button>().interactable = true;
            }
            else
            {
                var textComponent = GetComponentInChildren<TMPro.TMP_Text>();
                string text = textComponent.text;
                textComponent.SetText(text + " - forced");
            }

        }
        else
        {
            GetComponent<UnityEngine.UI.Button>().interactable = false;
        }
    }
}
