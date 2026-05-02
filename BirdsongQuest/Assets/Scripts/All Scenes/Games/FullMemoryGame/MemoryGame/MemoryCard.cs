using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Assets.Scripts.All_Scenes.Games.FullMemoryGame.MemoryGame
{
    public class MemoryCard : MonoBehaviour, IMemoryCard
    {
        [SerializeField] private GameObject frontRoot;
        [SerializeField] private GameObject backRoot;
        [SerializeField] private Button button;

        [SerializeField] private List<AudioClip> flipClips;


        [Header("Animation Settings")]
        [SerializeField] private float flipDuration = 0.5f;
        [SerializeField] private Ease flipEase = Ease.InOutQuad;

        private Bird bird;
        private bool isFrontShown;
        private Sequence flipSeq;

        public static event Action<MemoryCard> OnCardClickedGlobal;

        public bool IsMatched { get; private set; }
        public string BirdId => bird.id;
        public CardType Type { get; private set; }



        private bool isFlipped = false;
        private bool isAnimating = false;


        private void OnDestroy()
        {
            SoundFXManager.Instance.StopAllBirdSounds();
        }
        public void Init(Bird bird, CardType type)
        {
            this.Type = type;
            this.bird = bird;

            frontRoot.SetActive(false);
            backRoot.SetActive(true);
            button.interactable = true;
            IsMatched = false;

            if(type == CardType.Image)
            {
                var img = frontRoot
                            .GetComponentsInChildren<Image>(true)
                            .First(i => i.gameObject.name == "BirdImage"); 
                if (img != null)
                    img.sprite = bird.image;

                var label = frontRoot
                                .GetComponentsInChildren<TMP_Text>(true)
                                .First(i => i.gameObject.name == "BirdName");
                if (label != null)
                    label.text = bird.displayName;
            }
            else if(type == CardType.Sound)
            {
                var img = frontRoot
                            .GetComponentsInChildren<Image>(true)
                            .First(i => i.gameObject.name == "BirdImage");
                var birdName = frontRoot
                                .GetComponentsInChildren<TMP_Text>(true)
                                .First(i => i.gameObject.name == "BirdName");
                img.gameObject.SetActive(false);
                birdName.gameObject.SetActive(false);

                var label = frontRoot
                            .GetComponentsInChildren<TMP_Text>(true)
                            .First(i => i.gameObject.name == "Label");

                
                if (label != null)
                    label.text = "Audio";

                label.gameObject.SetActive(true);
            }
            else if(type == CardType.Name)
            {
                var label = frontRoot.GetComponentInChildren<TMP_Text>(true);
                if (label != null)
                    label.text = bird.displayName;
            }
            else
            {
                throw new NotImplementedException($"CardType {type} not implemented in MemoryCard.Init");
            }
        }

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(OnCardClicked);

            // Nur Rückseite sichtbar am Anfang
            frontRoot.SetActive(false);
            backRoot.SetActive(true);
        }

        private void OnCardClicked()
        {
            if (isAnimating || isFrontShown) 
                return;
            OnCardClickedGlobal?.Invoke(this);
        }

        public void Flip(bool showFront)
        {
            if (!isAnimating)
                StartCoroutine(FlipAnimation(showFront));
        }
        private IEnumerator FlipAnimation(bool showFront)
        {
            isAnimating = true;
            int randomClipIndex = Random.Range(0, flipClips.Count);
            UIEvents.RequestSound(flipClips[randomClipIndex], transform, 1f);
            // 1️⃣ Erste Hälfte (0° → 90°)
            yield return transform
                .DORotate(new Vector3(0, 90, 0), flipDuration / 2)
                .SetEase(flipEase)
                .WaitForCompletion();

            

            // 2️⃣ Seiten wechseln
            isFrontShown = showFront;
            frontRoot.SetActive(isFrontShown);
            backRoot.SetActive(!isFrontShown);

            // 3️⃣ Zweite Hälfte (90° → 0°)
            yield return transform
                .DORotate(Vector3.zero, flipDuration / 2)
                .SetEase(flipEase)
                .WaitForCompletion();

            transform.rotation = Quaternion.Euler(0, isFlipped ? 180 : 0, 0);
            isAnimating = false;

            if(Type == CardType.Sound && isFrontShown)
            {
                int randomBirdCallIndex = Random.Range(0, bird.calls.Length);
                UIEvents.RequestBirdSound(bird.calls[randomBirdCallIndex], transform, 1f);
            }
        }

        public void MarkAsFound()
        {
            IsMatched = true;
            button.interactable = false;
        }

        public void AddFoundCounter()
        {
            this.bird.AddCorrect();
        }

        public void MarkBirdAsFoundForCollectionBook()
        {
           this.bird.MarkAsFoundInGame(GameId.Mamory);
        }

    }


    
}
