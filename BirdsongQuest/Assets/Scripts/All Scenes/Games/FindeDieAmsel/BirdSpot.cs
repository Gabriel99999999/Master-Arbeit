using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.All_Scenes.Games.FindeDieAmsel
{
    public class BirdSpot : MonoBehaviour, IPointerClickHandler
    {
        [Header("UI")]
        [SerializeField] private TMP_Text _birdName;
        [SerializeField] private Image _birdImage;     // das Bird Sprite (UI Image)
        [SerializeField] private Image _borderImage;   // Rahmen (UI Image)

        [Header("Reveal Animation")]
        [SerializeField] private float hiddenY = -90f;
        [SerializeField] private float shownY = 0f;
        [SerializeField] private float revealDuration;

        [Header("Border Colors")]
        [SerializeField] private Color neutralBorder = new Color(1, 1, 1, 0.6f);
        [SerializeField] private Color correctBorder = Color.green;
        [SerializeField] private Color wrongBorder = Color.red;

        public Bird Bird { get; private set; } // null => leerer spot
        private FindeDieAmselGameController _controller;
        private RectTransform _birdRt;
        private Tween _revealTween;

        public string BirdId => Bird != null ? Bird.id : string.Empty;
        public bool HasBird => Bird != null;

        public void BindController(FindeDieAmselGameController controller)
        {
            _controller = controller;
        }

        private void Awake()
        {
            if (_birdImage != null)
                _birdRt = _birdImage.rectTransform;

            ResetVisual();
        }

        public void InitBird(Bird bird)
        {
            Bird = bird;

            if (_birdImage != null && bird != null)
            {
                _birdImage.sprite = bird.image;
                _birdImage.preserveAspect = true;
            }
            if (_birdName != null && bird != null)
            {
                _birdName.text = bird.displayName;
            }

            HideInstant();
            SetBorderNeutral();
            gameObject.SetActive(true);
        }

        public void SetEmpty()
        {
            Bird = null;
            HideInstant();
            SetBorderNeutral();
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }

        public void ResetVisual()
        {
            _revealTween?.Kill();
            HideInstant();
            SetBorderNeutral();
        }

        private void HideInstant()
        {
            if (_birdRt == null) return;

            _birdRt.localRotation = Quaternion.Euler(0f, -90f, 0f);
            _birdName.gameObject.SetActive(false);
        }

        public void RevealIfHasBird()
        {
            if (!HasBird || _birdRt == null) return;

            _revealTween?.Kill();
            _birdName.gameObject.SetActive(true);
            _birdRt.localRotation = Quaternion.Euler(0f, -90f, 0f);
            _revealTween = _birdRt.DOLocalRotate(new Vector3(0f, 0f, 0f), revealDuration)
                            .SetEase(Ease.OutCubic);
        }

        public void SetBorderNeutral() => SetBorderColor(neutralBorder);
        public void SetBorderCorrect() => SetBorderColor(correctBorder);
        public void SetBorderWrong() => SetBorderColor(wrongBorder);

        private void SetBorderColor(Color c)
        {
            if (_borderImage == null) return;
            _borderImage.enabled = true;
            _borderImage.color = c;
        }

        // Click Handling: Unity liefert clickCount, aber zuverlässig ist:
        // clickCount==2 für double click (wenn OS/Unity timing passt).
        // Fürs Spiel reicht das meistens.
        public void OnPointerClick(PointerEventData eventData)
        {
            if (_controller == null) return;

            if (eventData.clickCount >= 2)
            {
                _controller.OnSpotDoubleClicked(this);
            }
            else
            {
                _controller.OnSpotSingleClicked(this);
            }
        }

    }
}
