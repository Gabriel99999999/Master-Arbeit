using Assets.Scripts.All_Scenes.Games.FullMemoryGame.Settings;
using Assets.Scripts.All_Scenes.Games.FullQuizGame.Settings;
using Assets.Scripts.All_Scenes.Games.TestScene;
using Assets.Scripts.Timer;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

namespace Assets.Scripts.All_Scenes.Games.FullQuizGame.QuizGame
{    
    public class EndTestGameController : MonoBehaviour
    {
        [SerializeField] private QuizGameUIController _uiController; // Referenz auf UI
        [SerializeField] private BaseTimer _stopwatch;
        [SerializeField] private BirdSet birdSet;
        [SerializeField] private AudioWaveform audioWaveform;

        [SerializeField] private AudioClip successClip;
        [SerializeField] private AudioClip errorClip;

        private Question[] _questions;
        private QuizAnswer[] _answers;
        
        private int _correctAnswersCounter = 0;
        private int _incorrectAnswersCounter = 0;
        private int _amountOfAnswers;
        private int _numberOfBirds = 0;
        private int _numberOfQuestions = 0;
        private bool _gameIsPaused;
        private bool _isCheckingCorrect = false;
        private int _currentQuestionIndex = 0;
        private List<Bird> _birdsToBuildTheQuestionsAndAnswers;
        private Question _currentQuestion => _questions[_currentQuestionIndex];
        private Bird[] _answerOptions;
        private float _questionStartTime = 0;
        private GameId gameId = GameId.Quiz;

        private void Start()
        {
            MyLogger.Instance.LogStartGame(
                "EndQuiz",
                "14",
                "8"
            );
            SoundFXManager.Instance.SetBirdSoundMode(false);
            if (_stopwatch != null)
            {
                _stopwatch.StartTimer();
            }
            else
            {
                Debug.LogError("Stopwatch is not assigned in QuizGameController");
            }
            InitializeGame();
            UpdateStatesInUI();
            StartGame();

        }

        private void Awake()
        {
            /*if (_settingsAnchor == null)
            {
                _settingsAnchor = Resources.Load<QuizSettingsAnchor>("Anchors/QuizSettingsAnchor");
                Debug.Log($"Anchor wurde automatisch geladen: {_settingsAnchor}");
            }
            if (!_settingsAnchor.IsSet)
            {
                Debug.LogError("quizSettings was not set");
                return;
            }*/

            
        }

        private void InitializeGame()
        {
            _numberOfBirds = 10;

            //initializing some values.
            _birdsToBuildTheQuestionsAndAnswers = new List<Bird>();
            _numberOfQuestions = _numberOfBirds;
            _questions = new Question[_numberOfQuestions];

            if (_numberOfBirds > birdSet.birds.Count)
            {
                Debug.LogError("Not enough birds in the bird set for the test.");
                return;
            }

            for (int i = 0; i < _numberOfBirds; i++)
            {
                _birdsToBuildTheQuestionsAndAnswers.Add(birdSet.birds[i]);
            }

            //Checking if the game runs with 4 or 3 answer options
            _amountOfAnswers = 4;
            if (_numberOfBirds < 4)
            {
                _uiController.RemoveLastAnswerOption();
                _amountOfAnswers = 3;
            }

            PrepareQuestions();
        }
        private void PrepareQuestions()
        {
            int index = -1;
            //for each bird there is one Question with Audio and one with Image
            foreach (var bird in _birdsToBuildTheQuestionsAndAnswers)
            {
                index++;
                _questions[index] = new Question(QuestionType.Audio, bird);
                //index++;
                //_questions[index] = new Question(QuestionType.Image, bird);

                //can be deleted when birdSet contains only as many birds as the level allows.
                /*if (index == _numberOfQuestions - 1)
                    break;*/
            }
        }

        private void OnEnable()
        {
            QuizAnswer.OnAnswerClickedGlobal += HandleAnswerClicked;
            _uiController.OnPauseButtonClicked += HandlePause;
            _uiController.OnEndButtonClicked += HandleEndGame;
        }

        private void OnDisable()
        {
            QuizAnswer.OnAnswerClickedGlobal -= HandleAnswerClicked;
            _uiController.OnPauseButtonClicked -= HandlePause;
            _uiController.OnEndButtonClicked -= HandleEndGame;
        }

        private void Update()
        {
            if (_stopwatch != null)
                _stopwatch.Tick(Time.deltaTime);
        }
      

        private void HandlePause()
        {
            _gameIsPaused = !_gameIsPaused;
            _uiController.SetButtonsInteractable(!_gameIsPaused);
            if (_gameIsPaused)
            {
                _uiController.ChangeToPlayButton();
                _stopwatch.Stop();
                SoundFXManager.Instance.StopAllBirdSounds();
            }
            else
            {
                _uiController.ChangeToPauseButton();
                _stopwatch.Continue();
            }  
        }

        private void HandleEndGame()
        {
            _stopwatch.Stop();
            SoundFXManager.Instance.StopAllBirdSounds();
            _uiController.SetButtonsInteractable(false);
            float time = _stopwatch.GetTime();

            //calculate percentage of correct answers
            float percentageCorrect = ((float)_correctAnswersCounter / _numberOfQuestions) * 100f;

            MyLogger.Instance.LogQuizEndGame(
                "EndQuiz",
                "14",   
                "8",
                time.ToString("F2"),
                _numberOfQuestions.ToString(),
                _correctAnswersCounter.ToString(),
                _incorrectAnswersCounter.ToString(),
                percentageCorrect.ToString("F2"),
                "There is no win in the EndQuiz"
            );


            SceneLoader.Instance.LoadSceneWithDelay("EndScene", 0.5f);
        }




        private void StartGame()
        {
            ShuffleUtil.ShuffleInPlace(_questions);
            AskNextQuestion();
        }
        private void AskNextQuestion()
        {
            //Check End Condition for the game
            if (_currentQuestionIndex >= _numberOfQuestions)
            {
                Debug.Log("Game Over! All questions answered.");
                HandleEndGame();
                return;
            }
            UpdateStatesInUI();
            SetUpNextQuestion();
        }
        private void SetUpNextQuestion()
        {
            audioWaveform.DisplayAudioClip(_currentQuestion.call);
            _answerOptions = SetUpTheAnswers();
            _questionStartTime = _stopwatch.GetTime();
            _uiController.DisplayQuestion(_currentQuestion, _answerOptions);
            _uiController.SetButtonsInteractable(true);
        }
        private Bird[] SetUpTheAnswers()
        {
            Bird[] answerOptions = new Bird[_amountOfAnswers];
            List<Bird> possibleBirdsToUseAsAnswer = new List<Bird>(_birdsToBuildTheQuestionsAndAnswers);

            //Remove the correct answer from the possible answers
            possibleBirdsToUseAsAnswer.Remove(_currentQuestion.Bird);

            int randomBirdIndex = 0;
            answerOptions[0] = _currentQuestion.Bird;
            for (int answerIndex = 1; answerIndex < _amountOfAnswers; answerIndex++)
            {
                randomBirdIndex = UnityEngine.Random.Range(0, possibleBirdsToUseAsAnswer.Count);
                answerOptions[answerIndex] = (possibleBirdsToUseAsAnswer[randomBirdIndex]);
                possibleBirdsToUseAsAnswer.RemoveAt(randomBirdIndex);
            }
            ShuffleUtil.ShuffleInPlace(answerOptions);
            return answerOptions;
        }

        private void HandleAnswerClicked(QuizAnswer clickedAnswer)
        {
            float currentTime = _stopwatch.GetTime();
            float timeNeededForTheQuestion = currentTime - _questionStartTime;

            _uiController.SetButtonsInteractable(false);
            if (_gameIsPaused) return;
            if (_isCheckingCorrect) return; // Doppelklickschutz
            _isCheckingCorrect = true;
            SoundFXManager.Instance.StopAllBirdSounds();
            
            bool isCorrect = clickedAnswer.BirdID == _currentQuestion.Bird.id;

            if (isCorrect)
            {
                _currentQuestion.Bird.MarkAsFoundInGame(gameId);
                Debug.Log($"✅ Correct: {clickedAnswer.AnswerText}");
                if (successClip != null)
                    UIEvents.RequestSound(successClip, transform, 1f);
                clickedAnswer.ChangeColor(Color.green);
                _correctAnswersCounter++;
            }
            else
            {
                Debug.Log($"❌ Wrong: {clickedAnswer.AnswerText}");
                if (errorClip != null)
                    UIEvents.RequestSound(errorClip, transform, 1f);
                clickedAnswer.ChangeColor(Color.red);
                _incorrectAnswersCounter++;
            }


            MyLogger.Instance.LogQuizGameQuestion(
                    "EndQuiz",
                    "14",
                    "8",
                    CurrentQuestionNumber().ToString(),
                    _currentQuestion.call.name,
                    _currentQuestion.Bird.displayName,
                    _answerOptions.Select(b => b.displayName).ToArray(),
                    clickedAnswer.AnswerText,
                    isCorrect,
                    timeNeededForTheQuestion.ToString("F2")
                );


            // kleine Pause, dann nächste Frage
            StartCoroutine(NextQuestionAfterDelay(1.0f)); 
        }

        private System.Collections.IEnumerator NextQuestionAfterDelay(float delay)
        {
            _currentQuestionIndex++;
            yield return new WaitForSeconds(delay);
            _isCheckingCorrect = false;
            AskNextQuestion();
        }

        private int CurrentQuestionNumber()
        {
            return _currentQuestionIndex + 1;
        }

        private void UpdateStatesInUI()
        {
            _uiController.UpdateCorrectAnswer(_correctAnswersCounter);
            _uiController.UpdateIncorrectAnswer(_incorrectAnswersCounter);
            _uiController.UpdateQuestionNumber(CurrentQuestionNumber(), _numberOfQuestions);
        }


    }
}

