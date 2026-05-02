using Assets.Scripts.All_Scenes.Games;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts.All_Scenes.CollectionBook
{
    public class CollectionBookElement : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private Image birdImage;
        //[SerializeField] private Transform starsRoot;
        [SerializeField] private GameIcon[] _gameIcons;

        [SerializeField] public TextMeshProUGUI birdNameText;

        [Header("Locked Look (optional)")]
        [SerializeField] private Material grayscaleMat; // dein UI/Grayscale Material

        private Image[] _stars;

        private Bird bird;

        


        private void Awake()
        {
            // Star-Images automatisch holen und stabil sortieren
            /*_stars = starsRoot.GetComponentsInChildren<Image>(true)
                .Where(i => i.transform.parent == starsRoot)        // nur direkte Kinder
                .OrderBy(i => i.transform.GetSiblingIndex())         // Star_1..Star_5
                .ToArray();*/
        }

        public void Refresh(Bird bird, bool unlockedByLevel)
        {
            this.bird = bird;
            birdImage.sprite = bird.image;
            birdImage.preserveAspect = true;

            birdNameText.text = bird.displayName;

            // Sterne: aus PlayerPrefs lesen
            int correctCount = PlayerPrefs.GetInt($"BirdCorrect_{bird.id}", 0);


            foreach (GameId gameId in Enum.GetValues(typeof(GameId)))
            {
                bool found = PlayerPrefs.GetInt($"BirdFound_{bird.id}_{(int)gameId}", 0) > 0;
                GameIcon gameIcon = _gameIcons.Where(icon => icon.gameId == gameId).First();
                gameIcon.gameObject.GetComponent<Image>().material = found ? null : grayscaleMat;
            }
             // nur zum testen)

            // Vogel optisch lock/unlock
            birdImage.material = correctCount>0 ? null : grayscaleMat;
            birdImage.color = Color.white;

            // Deine Logik:
            // 0 -> alles grau (und eigentlich locked)
            // 1 -> unlock ohne Sterne
            // 2 -> 1 Stern ...
            /*int starsOn = Mathf.Clamp(correctCount-1, 0, 5);//-1 damit beim 1. mal richtig haben nur das bild freigeschalten wird aber nicht der erste stern 

            for (int i = 0; i < _stars.Length; i++)
            {
                bool on = unlockedByLevel && i < starsOn;
                _stars[i].color = on ? Color.yellow : Color.gray;

            }*/
        }

        public void PlaySound()
        {
            int indexOfCallToPlay = UnityEngine.Random.Range(0, bird.calls.Length);
            AudioClip clip = bird.calls[indexOfCallToPlay];
            SoundFXManager.Instance.StopAllBirdSounds();
            UIEvents.RequestBirdSound(clip, transform, 1f);
        }
    }
}
