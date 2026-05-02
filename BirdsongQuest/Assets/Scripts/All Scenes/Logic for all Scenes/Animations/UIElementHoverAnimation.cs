using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float duration = 0.25f;
    //[SerializeField] private Color hoverColor = new Color(0.7f, 1f, 0.7f);
    //[SerializeField] private Color normalColor = Color.white;

    private Vector3 originalScale;
    private Image image;

    private void Start()
    {
        originalScale = transform.localScale;
        image = GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(originalScale * hoverScale, duration).SetEase(Ease.OutBack);
        //if (image) image.DOColor(hoverColor, duration);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(originalScale, duration).SetEase(Ease.InBack);
        //if (image) image.DOColor(normalColor, duration);
    }

    private void OnDisable()
    {
        // 💡 Stoppt alle aktiven Tweens, wenn das Objekt deaktiviert oder entladen wird.
        transform.DOKill();
    }

    private void OnDestroy()
    {
        // 💡 doppelte Sicherheit, falls OnDisable nicht aufgerufen wurde (z. B. bei abruptem SceneUnload)
        transform.DOKill();
    }
}
