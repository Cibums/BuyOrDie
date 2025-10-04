using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string TooltipText;
    private Transform tooltipUIInstance;

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltipUIInstance = Instantiate(GameController.Instance.TooltipUIPrefab, transform).transform;
        tooltipUIInstance.gameObject.GetComponentInChildren<TMP_Text>().SetText(TooltipText);
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
