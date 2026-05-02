using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.All_Scenes.Games.FullQuizGame.QuizGame
{
    public class QuizGameUIController : MonoBehaviour
    {
        [Header("States Display")]
        [SerializeField] private TextMeshProUGUI _correctAnswersText;
        [SerializeField] private TextMeshProUGUI _incorrectAnswersText;
        [SerializeField] private TextMeshProUGUI _questionNumberText;

        [Header("Question Display")]
        [SerializeField] private TextMeshProUGUI _questionText;
        [SerializeField] private GameObject _imageContainer;
        [SerializeField] private GameObject _soundContainer;
        [SerializeField] private AudioWaveform _waveform;
        [SerializeField] private Image _pauseIcon;
        [SerializeField] private Image _playIcon;

        [Header("Answer Buttons (in Inspector zuweisen)")]
        [SerializeField] private List<QuizAnswer> _answerButtons = new List<QuizAnswer>();

        [Header("PlayButton")]
        [SerializeField] private Button _birdSongPlayBtn;

        public event Action OnPauseButtonClicked;
        public event Action OnEndButtonClicked;

        public void RemoveLastAnswerOption()
        {
            _answerButtons[_answerButtons.Count - 1].gameObject.SetActive(false);
            _answerButtons.RemoveAt(_answerButtons.Count - 1);
        }
        public void DisplayQuestion(Question currentQuestion, Bird[] answerOptions)
        {
            // 🧩 Frage anzeigen
            _questionText.text = currentQuestion.question;

            // 🔊 oder 🖼 je nach Frage-Typ
            if (currentQuestion.questionType == QuestionType.Image)
            {
                _imageContainer.SetActive(true);
                _soundContainer.SetActive(false);

                var img = _imageContainer.GetComponentInChildren<Image>();
                img.sprite = currentQuestion.Bird.image;
            }
            else
            {
                _imageContainer.SetActive(false);
                _soundContainer.SetActive(true);

                
                _waveform.DisplayAudioClip(currentQuestion.call);
            }

            // 🧱 Antwort-Buttons updaten
            for (int i = 0; i < _answerButtons.Count; i++)
            {
                if (i < answerOptions.Length)
                {
                    _answerButtons[i].Init(answerOptions[i]);
                }
            }
        }

        public void OnPauseOrPlay()
        {
            OnPauseButtonClicked?.Invoke();
        }

        public void ChangeToPlayButton()
        {
            _pauseIcon.gameObject.SetActive(false);
            _playIcon.gameObject.SetActive(true);
        }
        public void ChangeToPauseButton()
        {
            _pauseIcon.gameObject.SetActive(true);
            _playIcon.gameObject.SetActive(false);
        }

        public void OnEndGame()
        {
            OnEndButtonClicked?.Invoke();
        }

        public void SetButtonsInteractable(bool interactable)
        {
            _birdSongPlayBtn.interactable = interactable;

            foreach (var button in _answerButtons)
            {
                if (button != null)
                    button.SetInteractable(interactable);
            }
        }

        public void UpdateCorrectAnswer(int correctAnswers)
        {
            _correctAnswersText.text = $"{correctAnswers}";
        }
        public void UpdateIncorrectAnswer(int incorrectAnswers)
        {
            _incorrectAnswersText.text = $"{incorrectAnswers}";
        }
        public void UpdateQuestionNumber(int questionNumber, int amountOfQuestions)
        {
            _questionNumberText.text = $"{questionNumber}/{amountOfQuestions}";
        }
    }
}
