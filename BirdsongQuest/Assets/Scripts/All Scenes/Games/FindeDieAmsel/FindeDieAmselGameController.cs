using Assets.Scripts.All_Scenes.Games.FullMemoryGame;
using Assets.Scripts.All_Scenes.Games.FullMemoryGame.MemoryGame;
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

namespace Assets.Scripts.All_Scenes.Games.FindeDieAmsel
{
    public class FindeDieAmselGameController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private BirdSet birdSet;

        [Header("UI")]
        [SerializeField] private FindeDieAmselUIController ui;

        [Header("Spots (10)")]
        [SerializeField] private List<BirdSpot> spots; // zieh hier deine 10 Spot-Objekte rein

        [Header("Stopwatch")]
        [SerializeField] private BaseTimer _stopwatch;

        [Header("Level Settings")]
        [SerializeField] private FindeDieAmselSettingsAnchor _settings; 

        [Header("emptyExtraFields")]
        [SerializeField] private int _emptyExtraFields = 2;

        [Header("Audio")]
        [SerializeField] private AudioClip[] emptyClickClips; // wind/leaves
        [SerializeField] private AudioClip successClip;
        [SerializeField] private AudioClip errorClip;

        [Header("Timings")]
        [SerializeField] private float nextQuestionDelay;

        private List<Bird> _selectableBirds;
        private List<Bird> _levelBirds;
        private List<Bird> _questionOrder;
        private int _questionIndex;
        private Bird _currentTarget;

        private bool _inputLocked;
        private int _correctAnswers = 0;
        private int _incorrectAnswers = 0;
        private int _numberOfBirds = 0;
        private int _numberOfQuestions => _numberOfBirds;
        private int _maxSpots = 7;
        private bool _gameIsPaused = false;
        private float _questionStartTime = 0;

        private int maxNumberOfBirdsVisibleOnTree = 5;


        private void OnEnable()
        {
            ui.OnPauseButtonClicked += HandlePause;
        }
        private void OnDisable()
        {
            ui.OnPauseButtonClicked -= HandlePause;
        }

        private void Update()
        {
            if (_stopwatch != null)
                _stopwatch.Tick(Time.deltaTime);
        }

        private void Awake()
        {
            if (_settings == null)
            {
                _settings = Resources.Load<FindeDieAmselSettingsAnchor>("Anchors/FindeDieAmselSettingsAnchor");
                Debug.Log($"Anchor wurde automatisch geladen: {_settings}");
            }

            if (!_settings.IsSet)
            {
                Debug.LogError("memorySettingsAnchor was not set");
                return;
            }

            foreach (var s in spots)
                s.BindController(this);
        }

        private void Start()
        {
            MyLogger.Instance.LogStartGame("Finde die Amsel", _settings.Item.levelNumberForBestTimeAndNextBird.ToString(), ((int)_settings.Item.Level).ToString());
            SoundFXManager.Instance.SetBirdSoundMode(false);
            if (_stopwatch != null)
            {
                _stopwatch.StartTimer();
            }
            else
            {
                Debug.LogError("Stopwatch is not assigned in MemoryGameController");
            }
            StartGame();
        }

        public void StartGame()
        {
            
            _inputLocked = false;
            _correctAnswers = 0;
            _incorrectAnswers = 0;

           _numberOfBirds = GetBirdCountForLevel(_settings.Item.Level);

            // Nimm die ersten birds aus dem BirdSet
            _levelBirds = birdSet.birds.Take(_numberOfBirds).ToList();

            _questionOrder = new List<Bird>(_levelBirds);

            ShuffleUtil.ShuffleInPlace(_questionOrder);

            _questionIndex = 0;

            ui.UpdateStates(_questionIndex+1, _numberOfBirds, _correctAnswers, _incorrectAnswers);
            StartQuestion();
        }

        private void HandlePause()
        {
            _gameIsPaused = !_gameIsPaused;
            if (_gameIsPaused)
            {
                _stopwatch.Stop();
                SoundFXManager.Instance.StopAllBirdSounds();
            }
            else
            {
                _stopwatch.Continue();
            }

            ui.SetPauseMenuActive(_gameIsPaused);
        }
        private int GetBirdCountForLevel(FindeDieAmselLevel level)
        {
            int numberOfBirds = level switch
            {
                FindeDieAmselLevel.level1 => 3,
                FindeDieAmselLevel.level2 => 4,
                FindeDieAmselLevel.level3 => 5,
                FindeDieAmselLevel.level4 => 6,
                FindeDieAmselLevel.level5 => 7,
                FindeDieAmselLevel.level6 => 8,
                FindeDieAmselLevel.level7 => 9,
                _ => 10
            };
            return numberOfBirds;
        }

        private int GetDecoyCount()
        {
            //maximal 2 decoys
            return Math.Max(maxNumberOfBirdsVisibleOnTree - _numberOfBirds, 2); 
        }

        private void StartQuestion()
        {
            ui.UpdateStates(_questionIndex + 1, _numberOfBirds, _correctAnswers, _incorrectAnswers);
            _inputLocked = false;
            _currentTarget = _questionOrder[_questionIndex];
            ui.SetQuestion(_currentTarget.GermanArticleString, _currentTarget.displayName);

            SetupSpotsForThisQuestion(_levelBirds);
        }

        private void SetupSpotsForThisQuestion(List<Bird> birdsToUseInLevel)
        {
            int birdCount = birdsToUseInLevel.Count;
            int decoys = GetDecoyCount();
            int activeSpotCount = Mathf.Min(_maxSpots, birdCount + decoys);

            // 1) alle deaktivieren/reset
            foreach (var s in spots)
            {
                s.ResetVisual();
                s.Deactivate();
            }

            // 2) wähle activeCount Spots zufällig
            var indices = Enumerable.Range(0, spots.Count).ToList();
            ShuffleUtil.ShuffleInPlace(indices);
            var visibleContainers = indices.Take(activeSpotCount).Select(i => spots[i]).ToList();



            // Vögel rein
            //TargetVogel = Questioned Bird wird entfernt und händisch hinzugefügt damit die richtige Antwort auf jeden fall vorkommt. Der rest ist dann random
            List<Bird> knowBirdsWithoutTarget = birdsToUseInLevel.Where(b => b.id != _currentTarget.id).ToList();
            ShuffleUtil.ShuffleInPlace(knowBirdsWithoutTarget);

            //restlichen Antworten auffüllen
            //Anzahl der restlichen Antworten ist entweder die Anzahl der bekannten Vögel -1 (da einer der Vögel der Target Vogel ist) oder die maximale Anzahl an Vögeln die angezeigt werden können -1 (da einer der Vögel der Target Vogel ist)
            int answerPossibilitiesToAdd = Math.Min((birdCount - 1), (maxNumberOfBirdsVisibleOnTree-1));
            _selectableBirds = knowBirdsWithoutTarget.Take(answerPossibilitiesToAdd).ToList();

            //Correct Answer hinzufügen
            _selectableBirds.Add(_currentTarget);

            int amountOfFieldsWithBirds = _selectableBirds.Count;
            for (int i = 0; i < amountOfFieldsWithBirds; i++)
                visibleContainers[i].InitBird(_selectableBirds[i]);

            // rest leer (decoys)
            for (int i = amountOfFieldsWithBirds; i < amountOfFieldsWithBirds+decoys; i++)
                visibleContainers[i].InitBird(null);

            // 4) content random auf activeSpots verteilen
            ShuffleUtil.ShuffleInPlace(visibleContainers);

            for (int i = 0; i < visibleContainers.Count; i++)
            {
                var spot = visibleContainers[i];
                spot.gameObject.SetActive(true);

                if (visibleContainers[i].HasBird)
                    spot.InitBird(visibleContainers[i].Bird);
                else
                    spot.SetEmpty();
            }

            _questionStartTime = _stopwatch.GetTime();
        }

        public void OnSpotSingleClicked(BirdSpot spot)
        {
            if (_inputLocked) return;

            if (spot.HasBird)
            {
                PlayRandomBirdCall(spot);
            }
            else
            {
                PlayEmptySound();
            }
        }

        public void OnSpotDoubleClicked(BirdSpot spot)
        {
            float currentTime = _stopwatch.GetTime();
            float timeNeededForTheQuestion = currentTime - _questionStartTime;
            if (_inputLocked) return;
            _inputLocked = true;

            bool correct = spot.HasBird && spot.BirdId == _currentTarget.id;

            if (correct)
            {
                spot.Bird.MarkAsFoundInGame(GameId.FindeDieAmsel);
                spot.Bird.AddCorrect();
                _correctAnswers++;
                spot.SetBorderCorrect();
                spot.RevealIfHasBird();
                PlayOneShot(successClip);
            }
            else
            {
                _incorrectAnswers++;
                spot.SetBorderWrong();

                // wenn ein falscher Vogel dort war: zeig ihn trotzdem (Feedback)
                spot.RevealIfHasBird();

                PlayOneShot(errorClip);
            }

            MyLogger.Instance.LogFindeDieAmselQuestion(
                    "Finde die Amsel",
                    _settings.Item.levelNumberForBestTimeAndNextBird.ToString(),
                    ((int)_settings.Item.Level).ToString(),
                    (_questionIndex + 1).ToString(),
                    _selectableBirds.Select(b => b.displayName).ToArray(),
                    _currentTarget.displayName,
                    spot.HasBird ? spot.Bird.displayName : "Field with no Bird was clicked",
                    correct.ToString(),
                    timeNeededForTheQuestion.ToString("F2")
                );

            StartCoroutine(NextQuestionAfterDelay());
        }

        public void OnBackgroundClicked()
        {
            if (_inputLocked) return;
            PlayEmptySound();
        }

        private IEnumerator NextQuestionAfterDelay()
        {
            yield return new WaitForSeconds(nextQuestionDelay);

            _questionIndex++;

            if (_questionIndex >= _questionOrder.Count)
            {
                EndGame();
                yield break;
            }

            StartQuestion();
        }

        private void EndGame()
        {
            int currentLevel = (int)_settings.Item.Level;
            _inputLocked = true;
            _stopwatch.Stop();
            bool win = false;
            
            float time = _stopwatch.GetTime();
            //logging muss noch gemacht werden
            //MyLogger.Instance.LogMemoryEndGame("MemoryGame", _memoryMode.ToString(), _settingsAnchor.Item.Level.ToString(), _moveCounter.ToString(), time.ToString("F2"));


            //check if win condition is met
            float percentageCorrect = ((float)_correctAnswers / _numberOfQuestions) * 100f;
            //Win:
            if (percentageCorrect >= 60f)
            {
                win = true;
                //save best time if its a new record
                string key = $"LevelTime_{_settings.Item.levelNumberForBestTimeAndNextBird}";
                float oldBestTime = PlayerPrefs.GetFloat(key, -1f);

                Debug.Log($"Level: {_settings.Item.levelNumberForBestTimeAndNextBird}");
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
            }
            MyLogger.Instance.LogFindeDieAmselEnd(
                    "Finde die Amsel",
                    _settings.Item.levelNumberForBestTimeAndNextBird.ToString(),
                    currentLevel.ToString(),
                    time.ToString("F2"),
                    _numberOfQuestions.ToString(),
                    _correctAnswers.ToString(), 
                    _incorrectAnswers.ToString(),
                    percentageCorrect.ToString("F2"),
                    win.ToString()
                );
            Bird birdToShow = null;
            if (_numberOfBirds < 10)
                birdToShow = birdSet.birds[_numberOfBirds];
            GameSessionManager.Instance.ReportResult(win, _settings.Item.levelNumberForBestTimeAndNextBird, birdToShow, _settings.Item.nextLevelAddsANewBird);
            SceneLoader.Instance.LoadSceneWithDelay("LevelScene", 0.5f);
        }

        private void PlayRandomBirdCall(BirdSpot spot)
        {
            // spot hat den Bird intern, wir brauchen hier aber nur die calls → nimm über BirdId nicht nötig.
            // Einfach: nimm den Sprite-Spot-Bird? => BirdSpot hat _bird private.
            // Sauber: Finde Bird aus _levelBirds anhand id:
            var bird = spot.Bird;
            if (bird == null || bird.calls == null || bird.calls.Length == 0)
            {
                PlayEmptySound();
                return;
            }

            int indexOfCallToPlay = Random.Range(0, bird.calls.Length);
            PlayOneShot(bird.calls[indexOfCallToPlay]);
        }

        private void PlayEmptySound()
        {
            if (emptyClickClips == null || emptyClickClips.Length == 0) return;
            int idx = Random.Range(0, emptyClickClips.Length);
            PlayOneShot(emptyClickClips[idx]);
        }

        private void PlayOneShot(AudioClip clip)
        {
            if (clip == null) return;
            UIEvents.RequestBirdSound(clip, transform, 1f);
        }
    }
}
