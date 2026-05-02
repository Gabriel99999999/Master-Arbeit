using Assets.Scripts.All_Scenes.Games.BirdChorGame;
using Assets.Scripts.All_Scenes.Games.TestScene;
using Assets.Scripts.Timer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Utils;
using Random = UnityEngine.Random;

namespace Assets.Scripts.All_Scenes.Games.OrderBirdChorGame
{
    public class OrderBirdChorController : MonoBehaviour
    {
        [SerializeField] private OrderBirdChorUIController _uiController; // Referenz auf UI
        [SerializeField] private BirdChorSettingsAnchor _settingsAnchor;
        [SerializeField] private BaseTimer _stopwatch;
        [SerializeField] private BirdSet birdSet;

        [SerializeField] private AudioClip successClip;
        [SerializeField] private AudioClip errorClip;

        private List<Bird> _correctOrderOfTheBirds;
        private int _numberOfSingingBirds;
        private int _knownBirds;
        private int maxRounds = 4;
        private int currentRound = 0;
        
        private int _points = 0;
        private bool _gameIsPaused = false;
        private Coroutine _playRoutine;
        private Bird _singingBird;
        private bool _commitingAnswer = false;
        private List<AudioClip> _soundsInCorrectOrder = new List<AudioClip>();
        private float _questionStartTime = 0;


        private void Awake()
        {
            if (_settingsAnchor == null)
            {
                _settingsAnchor = Resources.Load<BirdChorSettingsAnchor>("Anchors/BirdChorSettingsAnchor");
                Debug.Log($"Anchor wurde automatisch geladen: {_settingsAnchor}");
            }

            if (!_settingsAnchor.IsSet)
            {
                Debug.LogError("BirdChorSettingsAnchor was not set");
                return;
            }
        }

        private void Start()
        {
            MyLogger.Instance.LogStartGame("BirdChorOrder", _settingsAnchor.Item.levelNumberForBestTimeAndNextBird.ToString(), ((int)_settingsAnchor.Item.Level).ToString());
            SoundFXManager.Instance.SetBirdSoundMode(false);
            if (_stopwatch != null)
            {
                _stopwatch.StartTimer();
            }
            else
            {
                Debug.LogError("Stopwatch is not assigned in MemoryGameController");
            }
            InitializeGame();
        }

        private void Update()
        {
            if (_stopwatch != null)
                _stopwatch.Tick(Time.deltaTime);
        }

        private void OnEnable()
        {
            _uiController.OnCommitBtnClicked += HandleCommitBtnClick;
            _uiController.OnPauseButtonClicked += HandlePause;
        }

        private void OnDisable()
        {
            _uiController.OnCommitBtnClicked -= HandleCommitBtnClick;
            _uiController.OnPauseButtonClicked -= HandlePause;
        }

        private void HandlePause()
        {
            _gameIsPaused = !_gameIsPaused;
            if (_gameIsPaused)
            {
                _uiController.ChangeToPlayButton();
                _stopwatch.Stop();
                SoundFXManager.Instance.StopAllBirdSounds();
                if (_playRoutine != null)
                    StopCoroutine(_playRoutine);
            }
            else
            {
                _uiController.ChangeToPauseButton();
                _stopwatch.Continue();
            }
        }

        private void InitializeGame()
        {
            InitializeStates();
            FillAnswerOptions();
        }

        private void HandleCommitBtnClick()
        {
            _uiController.SetSoundButtonsInteractable(false);
            if (_commitingAnswer)
                return;
            _commitingAnswer = true;
            StartCoroutine(CheckAnswers(_uiController.GetBirdsOrder()));
        }

        private void InitializeStates()
        {
            _numberOfSingingBirds = 4;
            if (_settingsAnchor.Item.Level < BirdChorLevel.level2)
            {
                _numberOfSingingBirds = 3;
                _uiController.RemoveOneSlot();
            }
                
            
            _knownBirds = _settingsAnchor.Item.Level switch
            {
                BirdChorLevel.level1 => 3,
                BirdChorLevel.level2 => 4,
                BirdChorLevel.level3 => 5,
                BirdChorLevel.level4 => 6,
                BirdChorLevel.level5 => 7,
                BirdChorLevel.level6 => 8,
                BirdChorLevel.level7 => 9,
                BirdChorLevel.level8 => 10,
                _ => 10

            };

    


            _uiController.UpdateQuestionNumber(currentRound + 1, maxRounds);
            _uiController.UpdatePointsNumber(_points);
        }

        private void FillAnswerOptions()
        {
            
            _uiController.UpdateQuestionNumber(currentRound + 1, maxRounds);
            _uiController.UpdatePointsNumber(_points);
            List<Bird> pool = birdSet.birds.Take(_knownBirds).ToList();
            ShuffleUtil.ShuffleInPlace(pool);

            _correctOrderOfTheBirds = pool.Take(_numberOfSingingBirds).ToList();
            
            currentRound++;
            _uiController.FillAnswerOptions(ShuffleUtil.ShuffledCopy(_correctOrderOfTheBirds), (int)_settingsAnchor.Item.Level);
            _uiController.UpdateSingingBirdNumber($"{1}/{_numberOfSingingBirds}");
            _uiController.ResetBorders();
            _singingBird = _correctOrderOfTheBirds[0];
            _soundsInCorrectOrder.Clear();
            foreach (var bird in _correctOrderOfTheBirds)
            {
                int randomBirdCallIndex = Random.Range(0, bird.calls.Length);
                AudioClip clip = bird.calls[randomBirdCallIndex];
                _soundsInCorrectOrder.Add(clip);
            }


            _questionStartTime = _stopwatch.GetTime();
            _uiController.SetSoundButtonsInteractable(true);
            _commitingAnswer = false;

        }
        public void PlayBirdSounds()
        {
            // Button ruft diese Methode auf
            if (_playRoutine != null)
                StopCoroutine(_playRoutine);

            _playRoutine = StartCoroutine(PlayBirdSoundsRoutine(0));
        }

        public void SkipCurrentBird()
        {
            int currentIndex = _correctOrderOfTheBirds.IndexOf(_singingBird) + 1;
            currentIndex = currentIndex % _correctOrderOfTheBirds.Count;

            if (_playRoutine != null)
                StopCoroutine(_playRoutine);

            _playRoutine = StartCoroutine(PlayBirdSoundsRoutine(currentIndex));
        }


        private IEnumerator PlayBirdSoundsRoutine(int indexOfBirdWhoShouldSing)
        {
            if(indexOfBirdWhoShouldSing >= _correctOrderOfTheBirds.Count || indexOfBirdWhoShouldSing<0)
            {
                Debug.LogError("indexOfBirdWhoShould Sing is out of range");
                yield break;
            }
            Debug.Log("Stopping bird sounds.");
            SoundFXManager.Instance.StopAllBirdSounds();

            for (int index = indexOfBirdWhoShouldSing; index < _correctOrderOfTheBirds.Count; index++)
            { 
                var bird = _correctOrderOfTheBirds[index];
                _singingBird = bird;
                _uiController.UpdateSingingBirdNumber($"{(index + 1)}/{_numberOfSingingBirds}");

                AudioClip clip = _soundsInCorrectOrder[index];

                UIEvents.RequestBirdSound(clip, transform, 1f);

                float timeToWait = Mathf.Min(clip.length, 12f);
                yield return new WaitForSeconds(timeToWait);

                SoundFXManager.Instance.StopAllBirdSounds();
                // optional kleine Pause:
                // yield return new WaitForSeconds(0.1f);
            }
        }

        public IEnumerator CheckAnswers(List<string> orderedIds)
        {
            float currentTime = _stopwatch.GetTime();
            float timeNeededForTheQuestion = currentTime - _questionStartTime;
            List<bool> correctnessList = new List<bool>();
            Debug.Log("StopAllBirdSounds: ");
            SoundFXManager.Instance.StopAllBirdSounds();
            List<(Bird bird, bool wasCorrect)> userGivenOrder = new List<(Bird, bool)>();
            bool postionCorrect = false;
            for (int i = 0; i < orderedIds.Count; i++)
            {
                postionCorrect = orderedIds[i] == _correctOrderOfTheBirds[i].id;
                correctnessList.Add(postionCorrect);

                userGivenOrder.Add((_correctOrderOfTheBirds.First(b => b.id == orderedIds[i]), postionCorrect));

                //Points calculation
                _points += postionCorrect ? 1 : 0;
            }

            MyLogger.Instance.LogBirdChorOrderQuestion(
                "BirdChorOrder",
                _settingsAnchor.Item.levelNumberForBestTimeAndNextBird.ToString(),
                ((int)_settingsAnchor.Item.Level).ToString(),
                currentRound.ToString(),
                _soundsInCorrectOrder.Select(clip => clip.name).ToArray(),
                userGivenOrder.ToArray(),
                timeNeededForTheQuestion.ToString());

            _uiController.Reveal(correctnessList);
            UIEvents.RequestSound(correctnessList.All(b=>b==true) ? successClip : errorClip, transform, 1f);
            _uiController.UpdatePointsNumber(_points);

            // 4) Fortschritt: pro korrekt erkannten Vogel hochzählen
            foreach (var bird in _correctOrderOfTheBirds)
            {
                bird.AddCorrect();
                bird.MarkAsFoundInGame(GameId.BirdChor);
            }

           
            

            if (currentRound < maxRounds)
            {
                yield return new WaitForSeconds(1f);
                FillAnswerOptions();
            }
            else
            {
                float succesRate = (_points / (float)(_numberOfSingingBirds * maxRounds)) * 100f;
                bool levelPassed = succesRate >= 60f;
                // Spiel beenden
                if (_stopwatch != null)
                {
                    _stopwatch.Stop();
                }
                else
                {
                    Debug.LogError("Stopwatch is not assigned in MemoryGameController");
                }

                string key = $"LevelTime_{_settingsAnchor.Item.levelNumberForBestTimeAndNextBird}";
                float oldBestTime = PlayerPrefs.GetFloat(key, -1f);
                float time = _stopwatch.GetTime();  
                Debug.Log($"Level: {_settingsAnchor.Item.levelNumberForBestTimeAndNextBird}");
                // Wenn keine alte Zeit existiert oder die neue besser ist → speichern
                if (oldBestTime < 0 || time < oldBestTime)
                {
                    PlayerPrefs.SetFloat(key, time);
                    PlayerPrefs.Save();
                    Debug.Log($"Neue Bestzeit gespeichert: {time} Sekunden");
                }
                else
                {
                    Debug.Log($"Alte Bestzeit bleibt: {oldBestTime} Sekunden");
                }

                Bird birdToShow = null;
                if (_knownBirds < 10)
                    birdToShow = birdSet.birds[_knownBirds];
                SoundFXManager.Instance.StopAllBirdSounds();
                MyLogger.Instance.LogBirdChorOrderEnd(
                    "BirdChorOrder",
                    _settingsAnchor.Item.levelNumberForBestTimeAndNextBird.ToString(),
                    ((int)_settingsAnchor.Item.Level).ToString(),
                    _stopwatch.GetTime().ToString(),
                    _points.ToString(),
                    (_numberOfSingingBirds * maxRounds).ToString(),
                    succesRate.ToString("F2"),
                    levelPassed.ToString());

                GameSessionManager.Instance.ReportResult(levelPassed, _settingsAnchor.Item.levelNumberForBestTimeAndNextBird, birdToShow, _settingsAnchor.Item.nextLevelAddsANewBird);
                SceneLoader.Instance.LoadSceneWithDelay("LevelScene", 1.2f);
            }
            // Optional: Buttons sperren / Next anzeigen etc.
        }


    }
}
