using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.All_Scenes.Games.FullQuizGame.QuizGame
{
    public class QuizAnswer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _answerLabel;
        [SerializeField] private Button _button;

        private Color startColor = new Color32(16, 79, 23, 255);


        public string BirdID { get; private set; }
        public string AnswerText { get; private set; }

        private Bird _bird;

        public static event Action<QuizAnswer> OnAnswerClickedGlobal;

        public void Init(Bird bird)
        {
            ChangeColor(startColor);
            this._bird = bird;
            BirdID = bird.id;
            AnswerText = bird.displayName;

            if (_answerLabel != null)
                _answerLabel.text = AnswerText;
        }

        private void Awake()
        {
            if (_button == null)
                _button = GetComponent<Button>();

            _button.onClick.AddListener(OnClicked);
        }

        private void OnClicked()
        {
            OnAnswerClickedGlobal?.Invoke(this);
        }

        public void SetInteractable(bool interactable)
        {
           _button.interactable = interactable;
        }

        public void ChangeColor(Color color)
        {
            _button.image.color = color;
        }

        public void AddFoundCounter()
        {
            _bird.AddCorrect();
        }
    }
}
