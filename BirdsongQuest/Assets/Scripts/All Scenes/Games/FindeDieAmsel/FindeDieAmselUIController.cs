using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.All_Scenes.Games.FindeDieAmsel
{
    public class FindeDieAmselUIController : MonoBehaviour
    {
        [SerializeField] private TMP_Text questionText;
        [SerializeField] private TMP_Text questionNumber;
        [SerializeField] private TMP_Text correctAnswersCounter;
        [SerializeField] private TMP_Text incorrectAnswersCounter;

        [SerializeField] private GameObject _pauseMenu;

        public event Action OnPauseButtonClicked;

        public void SetQuestion(string article, string birdName)
        {
            if (questionText != null)
                questionText.text = $"Finde {article} {birdName}:";
        }

        public void UpdateStates(int currentQuestion, int amountOfQuestions, int correctAnswersCounter, int incorrectAnswersCounter)
        {
            questionNumber.text = $"{currentQuestion}/{amountOfQuestions}";
            this.correctAnswersCounter.text = correctAnswersCounter.ToString();
            this.incorrectAnswersCounter.text = incorrectAnswersCounter.ToString();
        }

        public void SetPauseMenuActive(bool isPaused)
        {
            _pauseMenu.SetActive(isPaused);
        }

        public void OnPauseOrPlay()
        {
            OnPauseButtonClicked?.Invoke();
        }
    }
}
