using UnityEngine;

public class ChoiceButtonBehaviour : MonoBehaviour
{
    public TradeAction Action;

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
}
