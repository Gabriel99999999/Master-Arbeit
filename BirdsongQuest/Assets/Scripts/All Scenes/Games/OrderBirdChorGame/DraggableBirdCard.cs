using Assets.Scripts.All_Scenes.Games.OrderBirdChorGame;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.All_Scenes.Games.OrderBirdChorGame
{

    public class DraggableBirdCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Header("Refs")]
        [SerializeField] private Canvas canvas;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("UI")]
        [SerializeField] private TMP_Text birdName;
        [SerializeField] private Image birdImage;
        [SerializeField] private Image border;

        public DropSlot CurrentSlot { get; set; }
        public string BirdId { get; private set; }

        private RectTransform _rt;

        private void Awake()
        {
            _rt = GetComponent<RectTransform>();
            if (!canvasGroup) canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
            if (!canvas) canvas = GetComponentInParent<Canvas>();
        }

        public void Setup(Bird bird)
        {
            BirdId = bird.id;
            if (birdName) birdName.text = bird.displayName;
            if (birdImage) birdImage.sprite = bird.image;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            canvasGroup.blocksRaycasts = false; // wichtig: damit Slot den Drop bekommt
            transform.SetParent(canvas.transform, true); // nach vorne ziehen
        }

        public void OnDrag(PointerEventData eventData)
        {
            _rt.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            canvasGroup.blocksRaycasts = true;

            // wenn nicht gedroppt -> zurück in alten Slot
            if (CurrentSlot != null && transform.parent == canvas.transform)
            {
                SnapTo(CurrentSlot.transform);
            }
        }

        public void SnapTo(Transform parent)
        {
            transform.SetParent(parent, false);
            _rt.anchoredPosition = Vector2.zero;
        }

        public void ChangeColor(Color color)
        {
            border.color = color;
        }
    }
}
