using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.All_Scenes.Games.OrderBirdChorGame
{
    public class OrderBirdChorUIController : MonoBehaviour
    {
        [SerializeField] private List<DropSlot> _dropSlots;
        [SerializeField] private Image _pauseIcon;
        [SerializeField] private Image _playIcon;
        [SerializeField] private GameObject _pauseMenu;
        [SerializeField] private Button _playBirdSoundBtn;
        [SerializeField] private Button _skipBirdBtn;

        [Header("States Display")]
        [SerializeField] private TextMeshProUGUI _pointsText;
        [SerializeField] private TextMeshProUGUI _questionNumberText;
        [SerializeField] private TextMeshProUGUI _singingBirdNumber;

        public event Action OnCommitBtnClicked;
        public event Action OnPauseButtonClicked;

        private int _maxSelectable = 2;
        private bool _suppressCallback;

        public void RemoveOneSlot()
        {
            _dropSlots[3].gameObject.SetActive(false);
            _dropSlots.RemoveAt(3);
        }
        public void OnPauseOrPlay()
        {
            OnPauseButtonClicked?.Invoke();
        }
        public void SetMaxSelectable(int max)
        {
            _maxSelectable = Mathf.Max(1, max);
        }

        public void OnCommit()
        {
            OnCommitBtnClicked.Invoke();
        }

        public void ChangeToPauseButton()
        {
            _pauseMenu.SetActive(false);
            _pauseIcon.gameObject.SetActive(true);
            _playIcon.gameObject.SetActive(false);
        }

        public void ChangeToPlayButton()
        {
            _pauseMenu.SetActive(true);
            _pauseIcon.gameObject.SetActive(false);
            _playIcon.gameObject.SetActive(true);
        }

        public void FillAnswerOptions(List<Bird> birds, int levelNumber)
        {
            DraggableBirdCard draggableBirdCardToSetup;
            for (int i = 0; i < _dropSlots.Count; i++)
            {
                if (i < birds.Count)
                {
                    draggableBirdCardToSetup = _dropSlots[i].GetComponentsInChildren<DraggableBirdCard>(true).FirstOrDefault();
                    draggableBirdCardToSetup.Setup(birds[i]);
                }
                else
                {
                    Debug.LogError("There are more drop slots than birds to setup for this level: " + levelNumber);
                }

            }
        }

        public List<string> GetBirdsOrder()
        {
            return _dropSlots
                .Select(slot =>
                {
                    var card = slot.GetComponentInChildren<DraggableBirdCard>(true);
                    return card.BirdId;
                })
                .ToList();
        }

        public void ResetBorders()
        {
            foreach (var slot in _dropSlots)
            {
                DraggableBirdCard card = slot.GetComponentsInChildren<DraggableBirdCard>(true).FirstOrDefault();
                if(card!=null) 
                    card.ChangeColor(Color.white);

            }
        }

        public void Reveal(List<bool> correctPositions)
        {
            for (int i = 0; i < _dropSlots.Count; i++)
            {
                var card = _dropSlots[i].GetComponentsInChildren<DraggableBirdCard>(true).FirstOrDefault();
                bool correct = correctPositions[i];
                card.ChangeColor(correct ? Color.green : Color.red);
            }
        }

        public void UpdateQuestionNumber(int questionNumber, int amountOfQuestions)
        {
            _questionNumberText.text = $"{questionNumber}/{amountOfQuestions}";
        }

        public void UpdatePointsNumber(int points)
        {
            _pointsText.text = points.ToString();
        }

        public void UpdateSingingBirdNumber(string birdNumber)
        {
            _singingBirdNumber.text = $"{birdNumber} singt gerade";
        }

        public void SetSoundButtonsInteractable(bool interactable)
        {
            _playBirdSoundBtn.interactable = interactable;
            _skipBirdBtn.interactable = interactable;
        }
    }
}
