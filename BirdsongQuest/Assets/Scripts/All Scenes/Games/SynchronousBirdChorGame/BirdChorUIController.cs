using Assets.Scripts.All_Scenes.Games.FullMemoryGame.MemoryGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.Multiplayer.Center.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.All_Scenes.Games.BirdChorGame
{
    public class BirdChorUIController : MonoBehaviour
    {
        private List<Answer> _answers;
        [SerializeField] private List<GameObject> _answerContainers;
        [SerializeField] private Image _pauseIcon;
        [SerializeField] private Image _playIcon;
        [SerializeField] private GameObject _pauseMenu;

        [Header("States Display")]
        [SerializeField] private TextMeshProUGUI _pointsText;
        [SerializeField] private TextMeshProUGUI _questionNumberText;

        public event Action OnCommitBtnClicked;
        public event Action OnPauseButtonClicked;

        private int _maxSelectable = 2;
        private bool _suppressCallback;
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
            GameObject answerContainerToFill = _answerContainers.Where(ac => ac.GetComponentsInChildren<Answer>(includeInactive: true).Length == birds.Count).FirstOrDefault();
            if (answerContainerToFill == null)
            {
                Debug.LogError("There is no answerContainer in the scene which has enough answerOptions for this level: "+levelNumber);
            }
            
            _answers = answerContainerToFill.GetComponentsInChildren<Answer>().ToList();
            foreach (var (bird, answer) in birds.Zip(_answers, (b, a) => (b, a)))
            {
                answer.Init(bird);
            }
            answerContainerToFill.SetActive(true);

            SetupSelectionLimit();
        }

        public List<string> GetSelectedBirdIds()
        {
            return _answers
                .Where(a => a.IsSelected)
                .Select(a => a.BirdId)
                .ToList();
        }

        public void ResetBorders()
        {
            foreach (var a in _answers)
            {
                Image border = a.GetComponentsInChildren<Image>(true)
                                .FirstOrDefault(i => i.gameObject.name == "Border");
                border.color = Color.white;
                border.gameObject.SetActive(true);
            }
        }

        public void Reveal(HashSet<string> correctIds)
        {
            foreach (var a in _answers)
            {
                bool selected = a.IsSelected;
                //only give feedback for selected answers
                if (selected)
                {
                    bool correct = correctIds.Contains(a.BirdId);
                    Image border = a.GetComponentsInChildren<Image>(true)
                                    .FirstOrDefault(i => i.gameObject.name == "Border");
                    border.color = correct ? Color.green : Color.red;
                    border.gameObject.SetActive(true);
                }
                
            }
        }

        private void SetupSelectionLimit()
        {
            foreach (var a in _answers)
            {
                a.checkbox.onValueChanged.RemoveAllListeners();
                a.checkbox.onValueChanged.AddListener(isOn => OnAnswerToggled(a, isOn));
            }

            RefreshInteractables();
        }

        private void OnAnswerToggled(Answer changed, bool isOn)
        {
            if (_suppressCallback) return;

            if (isOn)
            {
                int selectedCount = _answers.Count(x => x.IsSelected);
                if (selectedCount > _maxSelectable)
                {
                    // zu viele -> sofort rückgängig
                    _suppressCallback = true;
                    changed.SetSelectedSilently(false);
                    _suppressCallback = false;

                    // optional: Sound/Hint anzeigen
                    // UIEvents.RequestSound(errorClip, transform, 1f);
                }
            }

            RefreshInteractables();
        }

        private void RefreshInteractables()
        {
            int selectedCount = _answers.Count(x => x.IsSelected);
            bool limitReached = selectedCount >= _maxSelectable;

            foreach (var a in _answers)
            {
                // Wenn Limit erreicht: nur die bereits selektierten bleiben klickbar (damit man abwählen kann)
                if (limitReached && !a.IsSelected) a.SetInteractable(false);
                else a.SetInteractable(true);
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

    }
}
