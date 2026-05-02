using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject tooltipRoot; // Panel
    [SerializeField] private TMP_Text tooltipText;   // Text
    [TextArea][SerializeField] private string text = "Sammelbuch öffnen";

    private void Awake()
    {
        if (tooltipRoot != null)
            tooltipRoot.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltipText != null)
            tooltipText.text = text;

        if (tooltipRoot != null)
            tooltipRoot.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltipRoot != null)
            tooltipRoot.SetActive(false);
    }
}
