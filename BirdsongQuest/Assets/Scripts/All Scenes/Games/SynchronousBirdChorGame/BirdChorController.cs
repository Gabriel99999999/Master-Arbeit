using Assets.Scripts.All_Scenes.Games.FullMemoryGame;
using Assets.Scripts.All_Scenes.Games.FullMemoryGame.MemoryGame;
using Assets.Scripts.All_Scenes.Games.FullQuizGame;
using Assets.Scripts.All_Scenes.Games.TestScene;
using Assets.Scripts.Timer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Assets.Scripts.All_Scenes.Games.BirdChorGame
{
    public class BirdChorController : MonoBehaviour
    {
        [SerializeField] private BirdChorUIController _uiController; // Referenz auf UI
        [SerializeField] private BirdChorSettingsAnchor _settingsAnchor;
        [SerializeField] private BaseTimer _stopwatch;
        [SerializeField] private BirdSet birdSet;

        [SerializeField] private AudioClip successClip;
        [SerializeField] private AudioClip errorClip;

        private int _numberOfSingingBirds;
        private int _knownBirds;
        private HashSet<string> _correctAnswersIds;
        private List<Bird> _correctBirds;
        private List<Bird> _wrongBirds;
        private List<AudioClip> _birdChor = new List<AudioClip>();
        private bool _syncronousSinging;

        private int maxRounds = 4;
        private int currentRound = 0;
        private bool _gameIsPaused = false;
        private List<List<Bird>> _allRightAnswerCombinations;
        private int _points = 0;
        private bool _commitingAnswer = false;
        private List<(Bird bird, bool isCorrect)> _answers;
        private float _questionStartTime = 0;

        private void Start()
        {
            MyLogger.Instance.LogStartGame("BirdChor - Syncron", _settingsAnchor.Item.levelNumberForBestTimeAndNextBird.ToString(), _settingsAnchor.Item.Level.ToString());
            SoundFXManager.Instance.SetBirdSoundMode(_syncronousSinging);
            if (_stopwatch != null)
            {
                _stopwatch.StartTimer();
            }
            else
            {
                Debug.LogError("Stopwatch is not assigned in MemoryGameController");
            }

            Debug.Log("BirdChorController Start finished: "+_stopwatch.GetTime());
            InitializeGame();
        }

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
            _syncronousSinging = _settingsAnchor.Item.Mode == BirdChorMode.synchronousSinging;
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
            }
            else
            {
                _uiController.ChangeToPauseButton();
                _stopwatch.Continue();
            }
        }

        private void HandleCommitBtnClick()
        {
            if (_commitingAnswer)
                return;
            _commitingAnswer = true;
            StartCoroutine(CheckAnswers(_uiController.GetSelectedBirdIds()));
        }

        private void Update()
        {
            if (_stopwatch != null)
                _stopwatch.Tick(Time.deltaTime);
        }
        private void InitializeGame()
        {
            InitializeStates();
            FillAnswerOptions();
        }

        private void InitializeStates()
        {
            if(_settingsAnchor.Item.Level < BirdChorLevel.level4)
            {
                Debug.LogError("BirdChorLevel must be at least level4");
                return;
            }
            //_memoryMode = _settingsAnchor.Item.Mode;
            _numberOfSingingBirds = _settingsAnchor.Item.Level switch
            {
                BirdChorLevel.level4 => 2,
                BirdChorLevel.level5 => 3,
                BirdChorLevel.level6 => 3,
                _ => 3
            };
            _knownBirds = _settingsAnchor.Item.Level switch
            {
                BirdChorLevel.level4 => 6,
                BirdChorLevel.level5 => 7,
                BirdChorLevel.level6 => 8,
                BirdChorLevel.level7 => 9,
                BirdChorLevel.level8 => 10,
                _ => 10

                
            };

            _allRightAnswerCombinations = GetCombinations(birdSet.birds.Take<Bird>(_knownBirds).ToList(), _numberOfSingingBirds);
            ShuffleUtil.ShuffleInPlace(_allRightAnswerCombinations);

            
            _uiController.UpdateQuestionNumber(currentRound + 1, maxRounds);
            _uiController.UpdatePointsNumber(_points);
        }

        private void FillAnswerOptions()
        {
            _uiController.UpdateQuestionNumber(currentRound + 1, maxRounds);
            _uiController.UpdatePointsNumber(_points);
            _birdChor.Clear();
            List<Bird> pool = birdSet.birds.Take(_knownBirds).ToList();
            ShuffleUtil.ShuffleInPlace(pool);

            var correctBirds = _allRightAnswerCombinations[currentRound];
            _wrongBirds = pool.Where(bird => !correctBirds.Contains(bird)).Take(2).ToList();
            currentRound++;
            _answers = new List<(Bird bird, bool isCorrect)>();
            _answers.AddRange(correctBirds.Select(b => (b, true)));
            _answers.AddRange(_wrongBirds.Select(b => (b, false)));
            ShuffleUtil.ShuffleInPlace(_answers);
            _correctBirds = correctBirds;
            _correctAnswersIds = correctBirds.Select(b => b.id).ToHashSet();

            _uiController.SetMaxSelectable(_numberOfSingingBirds);
            _uiController.FillAnswerOptions(_answers.Select(a=>a.bird).ToList(), (int)_settingsAnchor.Item.Level);
            _uiController.ResetBorders();
            foreach (var bird in _correctBirds)
            {
                int randomBirdCallIndex = Random.Range(0, bird.calls.Length);
                _birdChor.Add(bird.calls[randomBirdCallIndex]);
            }
            _commitingAnswer = false;
            _questionStartTime = _stopwatch.GetTime();
        }

        public void PlayBirdSounds()
        {
            Debug.Log("Stopping bird sounds.");
            SoundFXManager.Instance.StopAllBirdSounds();
            float minLen = _birdChor.Min(c => c.length);
            foreach (var call in _birdChor)
            {
                UIEvents.RequestBirdSound(call, transform, 1f);
            }
            Debug.Log("MinLen: " + minLen);
            StartCoroutine(StopBirdsAfterSeconds(minLen));
        }

        private IEnumerator StopBirdsAfterSeconds(float seconds)
        {
            Debug.Log("StopBirdsAfterSeconds: " + seconds);
            yield return new WaitForSeconds(seconds);
            SoundFXManager.Instance.StopAllBirdSounds();
        }


        public IEnumerator CheckAnswers(List<string> selectedIds)
        {
            
            float currentTime = _stopwatch.GetTime();
            float timeNeededForTheQuestion = currentTime - _questionStartTime;
            Debug.Log("StopAllBirdSounds: ");
            SoundFXManager.Instance.StopAllBirdSounds();
            var selectedSet = selectedIds.ToHashSet();

            // 1) korrekt erkannte Vögel (nur die richtigen, die auch angekreuzt wurden)
            List<Bird> correctlyRecognizedBirds = new List<Bird>();
            List<Bird> wrongSelectedBirds = new List<Bird>();
            List<bool> wasCorrect = new List<bool>();
            List<Bird> selectedBirds = new List<Bird>();
            // 2) falsche Auswahl prüfen
            foreach (var id in selectedSet)
            {
                Bird bird;
                if (_correctAnswersIds.Contains(id))
                {
                    bird = _correctBirds.First(b => b.id == id);
                    correctlyRecognizedBirds.Add(bird);
                    wasCorrect.Add(true);

                }
                else
                {
                    bird = _wrongBirds.First(b => b.id == id);
                    wrongSelectedBirds.Add(bird);
                    wasCorrect.Add(false);
                }
                selectedBirds.Add(bird);

            }
           

           
            bool anyWrongSelected = wrongSelectedBirds.Any();

            // 3) fehlen noch richtige?
            List<Bird> birdsWhichAreNotSelectedButShould = _correctBirds
                .Where(b => !selectedSet.Contains(b.id))
                .ToList();
            bool allCorrectSelected = birdsWhichAreNotSelectedButShould.Count() == 0;

            //Points calculation
            _points = _points + correctlyRecognizedBirds.Count;// - wrongSelectedBirds.Count;// - birdsWhichAreNotSelectedButShould.Count;
            bool roundPassed = !anyWrongSelected && allCorrectSelected;
            

            // UI Reveal
            _uiController.Reveal(_correctAnswersIds);

            _uiController.UpdatePointsNumber(_points);

            // 4) Fortschritt: pro korrekt erkannten Vogel hochzählen
            foreach (var bird in correctlyRecognizedBirds)
            {
                bird.AddCorrect();
                bird.MarkAsFoundInGame(GameId.BirdChor);
            }
            MyLogger.Instance.LogBirdChorSyncQuestion(
                "BirdChor - Syncron",
                _settingsAnchor.Item.levelNumberForBestTimeAndNextBird.ToString(),
                _settingsAnchor.Item.Level.ToString(),
                (currentRound).ToString(),
                _birdChor.Select(call => call.name).ToArray(),
                _answers.Select(a => a.bird.displayName).ToArray(),
                selectedBirds.Select(b => b.displayName).ToArray(),
                wasCorrect.ToArray(),
                timeNeededForTheQuestion.ToString("F2")

            );
            // Optional: Sound Feedback
            UIEvents.RequestSound(roundPassed ? successClip : errorClip, transform, 1f);

            if(currentRound < maxRounds)
            {
                yield return new WaitForSeconds(1f);
                FillAnswerOptions();
            }
            else
            {
                float succesRate = (_points / (float)(_numberOfSingingBirds * maxRounds)) * 100f;
                bool levelPassed = succesRate >= 60f; // z.B. 70% der Punkte zum Bestehen
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
                MyLogger.Instance.LogBirdChorSyncEnd(
                    "BirdChor - Syncron",
                    _settingsAnchor.Item.levelNumberForBestTimeAndNextBird.ToString(),
                    _settingsAnchor.Item.Level.ToString(),
                    _stopwatch.GetTime().ToString("F2"),
                    _points.ToString(),
                    (_numberOfSingingBirds * maxRounds).ToString(),
                    succesRate.ToString("F2"),
                    levelPassed.ToString()
                    );
                yield return new WaitForSeconds(0.5f);
                GameSessionManager.Instance.ReportResult(levelPassed, _settingsAnchor.Item.levelNumberForBestTimeAndNextBird, birdToShow, _settingsAnchor.Item.nextLevelAddsANewBird);
                SceneLoader.Instance.LoadSceneWithDelay("LevelScene", 1.2f);
            }
            // Optional: Buttons sperren / Next anzeigen etc.
        }

        public static List<List<T>> GetCombinations<T>(List<T> items, int k)
        {
            ShuffleUtil.ShuffleInPlace(items);
            var result = new List<List<T>>();
            if (k <= 0 || k > items.Count) return result;

            void Backtrack(int start, List<T> current)
            {
                if (current.Count == k)
                {
                    result.Add(new List<T>(current));
                    return;
                }

                for (int i = start; i < items.Count; i++)
                {
                    current.Add(items[i]);
                    Backtrack(i + 1, current);
                    current.RemoveAt(current.Count - 1);
                }
            }

            Backtrack(0, new List<T>());
            return result;
        }
    }
}
