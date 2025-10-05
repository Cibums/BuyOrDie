using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string TooltipText;
    private RectTransform tooltipUIInstance;

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltipUIInstance = Instantiate(GameController.Instance.TooltipUIPrefab, transform).transform as RectTransform;
        tooltipUIInstance.gameObject.GetComponentInChildren<TMP_Text>().SetText(TooltipText);
        tooltipUIInstance.sizeDelta = new Vector2(25 * TooltipText.Length, tooltipUIInstance.sizeDelta.y);
        tooltipUIInstance.localPosition = new Vector3(12.5f * TooltipText.Length + 50, -100, 0);
        Debug.Log("Mouse entereed " + gameObject.name);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltipUIInstance != null)
        {
            Destroy(tooltipUIInstance.gameObject);
            tooltipUIInstance = null;
        }
        Debug.Log("Mouse exited " + gameObject.name);
    }
}
