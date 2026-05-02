using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.All_Scenes.Games.OrderBirdChorGame
{

    public class DropSlot : MonoBehaviour, IDropHandler
    {
        public DraggableBirdCard CurrentCard { get; private set; }

        private void Awake()
        {
            // falls im Editor schon eine Karte als Kind drin ist
            CurrentCard = GetComponentInChildren<DraggableBirdCard>(true);
            if (CurrentCard != null) CurrentCard.CurrentSlot = this;

            // sicherstellen: Slot kann Raycasts bekommen
            var img = GetComponent<Image>();
            if (img == null)
            {
                img = gameObject.AddComponent<Image>();
                img.color = new Color(1, 1, 1, 0); // transparent
            }
            img.raycastTarget = true;
        }

        public void OnDrop(PointerEventData eventData)
        {
            var dropped = eventData.pointerDrag?.GetComponent<DraggableBirdCard>();
            if (dropped == null) return;

            var fromSlot = dropped.CurrentSlot;
            var toSlot = this;

            if (fromSlot == null) return;

            // Wenn ich auf denselben Slot droppe -> einfach zurück snappen
            if (fromSlot == toSlot)
            {
                dropped.SnapTo(toSlot.transform);
                return;
            }

            // SWAP: Karte die hier liegt (falls vorhanden) zurück in fromSlot
            var toCard = CurrentCard;

            if (toCard != null)
            {
                toCard.SnapTo(fromSlot.transform);
                fromSlot.SetCard(toCard);
            }
            else
            {
                fromSlot.ClearCard();
            }

            // Dropped in diesen Slot
            dropped.SnapTo(toSlot.transform);
            SetCard(dropped);
        }

        private void SetCard(DraggableBirdCard card)
        {
            CurrentCard = card;
            card.CurrentSlot = this;
        }

        private void ClearCard()
        {
            CurrentCard = null;
        }
    }
}