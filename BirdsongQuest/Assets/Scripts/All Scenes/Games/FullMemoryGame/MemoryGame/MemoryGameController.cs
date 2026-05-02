using Assets.Scripts.Timer;
using UnityEngine;
using Utils;
using System.Collections.Generic;
using Assets.Scripts.All_Scenes.Games.FullMemoryGame.Settings;
using Assets.Scripts.All_Scenes.Games.TestScene;

namespace Assets.Scripts.All_Scenes.Games.FullMemoryGame.MemoryGame
{
    public class MemoryGameController : MonoBehaviour
    {
        [SerializeField] private GameFinishedEventSO _gameFinishedEvent;
        [SerializeField] private MemoryGameUIController _uiController; // Referenz auf UI
        [SerializeField] private MemorySettingsAnchor _settingsAnchor;
        [SerializeField] private BaseTimer _stopwatch;
        [SerializeField] private BirdSet birdSet;

        [SerializeField] private AudioClip successClip;
        [SerializeField] private AudioClip errorClip;

        private List<IMemoryCard> _flippedCards = new List<IMemoryCard>();
        private int _foundPairs = 0;
        private int _moveCounter = 0;
        private int _numberOfBirds = 0;
        private bool _gameIsPaused = false;
       // private MemoryMode _memoryMode;
        private bool _isCheckingMatch = false;
        private int _movesSinceTheLastPairWasFound = 0;
        private float _timeAtLastMatch = 0f;
        private float _timeSinceLastMatch = 0f;


        private void Start()
        {
            MyLogger.Instance.LogStartGame("Memory", _settingsAnchor.Item.levelNumberForBestTimeAndNextBird.ToString(), ((int)_settingsAnchor.Item.Level).ToString());
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

        private void Awake()
        {
            if (_settingsAnchor == null)
            {
                _settingsAnchor = Resources.Load<MemorySettingsAnchor>("Anchors/MemorySettingsAnchor");
                Debug.Log($"Anchor wurde automatisch geladen: {_settingsAnchor}");
            }
  
            if (!_settingsAnchor.IsSet)
            {
                Debug.LogError("memorySettingsAnchor was not set");
                return;
            }

            
        }

        private void OnEnable()
        {
            MemoryCard.OnCardClickedGlobal += OnCardFlipped;
            _uiController.OnPauseButtonClicked += HandlePause;
            _uiController.OnEndButtonClicked += HandleEndGame;
        }

        private void OnDisable()
        {
            MemoryCard.OnCardClickedGlobal -= OnCardFlipped;
            _uiController.OnPauseButtonClicked -= HandlePause;
            _uiController.OnEndButtonClicked -= HandleEndGame;
        }

        private void Update()
        {
            if (_stopwatch != null)
                _stopwatch.Tick(Time.deltaTime);
        }
        private void InitializeGame()
        {
            InitializeStates();
            BuildBoard();
        }
        private void BuildBoard()
        {
            List<(Bird bird, CardType type)> cards = new List<(Bird bird, CardType type)>();
            for (int index = 0; index < _numberOfBirds; index++)
            {
                var bird = birdSet.birds[index];
                cards.Add((bird, CardType.Image));

                cards.Add((bird, CardType.Sound));
                /*if (_memoryMode == MemoryMode.NameAndImage)
                {
                    cards.Add((bird, CardType.Name));
                }
                else if (_memoryMode == MemoryMode.SoundAndImage)
                {
                    cards.Add((bird, CardType.Sound));
                }*/


            }
            ShuffleUtil.ShuffleInPlace(cards);
            _uiController.CreateBoard(cards);
          
        }
        private void InitializeStates()
        {
            //_memoryMode = _settingsAnchor.Item.Mode;
            _numberOfBirds = _settingsAnchor.Item.Level switch
            {
                MemoryLevel.level1 => 3,
                MemoryLevel.level2 => 4,
                MemoryLevel.level3 => 5,
                MemoryLevel.level4 => 6,
                MemoryLevel.level5 => 7,
                MemoryLevel.level6 => 8,
                MemoryLevel.level7 => 9,
                _ => 10
            };

            _uiController.UpdateLevelText((int)_settingsAnchor.Item.Level);
            _uiController.UpdatePairText(_foundPairs, _numberOfBirds);
            _uiController.UpdateMoveCounter(_moveCounter);
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

            _uiController.SetPauseMenuActive(_gameIsPaused);
        }

   

        private void HandleEndGame()
        {
            
            _stopwatch.Stop();
            
            float time = _stopwatch.GetTime();
            //MyLogger.Instance.LogMemoryEndGame("MemoryGame", _memoryMode.ToString(), _settingsAnchor.Item.Level.ToString(), _moveCounter.ToString(), time.ToString("F2"));
            int currentLevel = (int)_settingsAnchor.Item.Level;
            string key = $"LevelTime_{_settingsAnchor.Item.levelNumberForBestTimeAndNextBird}";
            float oldBestTime = PlayerPrefs.GetFloat(key, -1f);

            Debug.Log($"Level: {currentLevel}");
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

            MyLogger.Instance.LogMemoryEndGame("Memory", _settingsAnchor.Item.levelNumberForBestTimeAndNextBird.ToString(), currentLevel.ToString(), _moveCounter.ToString(), time.ToString("F2"));


            Bird birdToShow = null;
            if (_numberOfBirds < 10)
                birdToShow = birdSet.birds[_numberOfBirds];
            SoundFXManager.Instance.StopAllBirdSounds();
            GameSessionManager.Instance.ReportResult(true, _settingsAnchor.Item.levelNumberForBestTimeAndNextBird, birdToShow, _settingsAnchor.Item.nextLevelAddsANewBird);
            SceneLoader.Instance.LoadSceneWithDelay("LevelScene", 0.5f);
        }

        private void OnCardFlipped(MemoryCard card)
        {
            if (_gameIsPaused) return;
            if (_isCheckingMatch) return; 
            if (_flippedCards.Contains(card)) return;
            if (_flippedCards.Count >= 2) return; // <— keine dritte Karte

            card.Flip(true);

            _flippedCards.Add(card);
            if(_flippedCards.Count == 1)
            {
                SoundFXManager.Instance.StopAllBirdSounds();
            }

            if (_flippedCards.Count == 2)
            {
                
                _moveCounter++;
                StartCoroutine(CheckMatch());
                _uiController.UpdateMoveCounter(_moveCounter);
            }
                
        }

        private System.Collections.IEnumerator CheckMatch()
        {
            _isCheckingMatch = true;
            yield return new WaitForSeconds(0.4f); // kleine Pause für Animation

            var first = _flippedCards[0];
            var second = _flippedCards[1];

            bool match = (first.BirdId == second.BirdId && first.Type != second.Type);

            
            float currentTime = _stopwatch.GetTime();
            if (match)
            {
                
                _foundPairs++;
                
            }
            _timeSinceLastMatch = currentTime - _timeAtLastMatch;

            string foundPair = match ? _foundPairs.ToString() : "No pair";


            MyLogger.Instance.LogMemoryGameMove(
                   "Memory",
                   _settingsAnchor.Item.levelNumberForBestTimeAndNextBird.ToString(),
                   _settingsAnchor.Item.Level.ToString(),
                   _moveCounter.ToString(),
                   foundPair,
                   match,
                   _movesSinceTheLastPairWasFound.ToString(),
                   _stopwatch.GetTime().ToString("F2"),
                   _timeSinceLastMatch.ToString("F2")
                );

            

            if (match)
            {
                _timeAtLastMatch = currentTime;
                _movesSinceTheLastPairWasFound = 0;
                // ✅ Paar gefunden
                
                _uiController.UpdatePairText(_foundPairs, _numberOfBirds);
                if (successClip != null)
                    UIEvents.RequestSound(successClip, transform, 1f);

                first.AddFoundCounter();
                first.MarkBirdAsFoundForCollectionBook();

                if (_foundPairs >= _numberOfBirds)
                    HandleEndGame();
            }
            else
            {
                
                _movesSinceTheLastPairWasFound++;
                // ❌ Falsches Paar → zurückflippen
                if (errorClip != null)
                    UIEvents.RequestSound(errorClip, transform, 1f);
                yield return new WaitForSeconds(errorClip.length);

                //SoundFXManager.Instance.StopAllBirdSounds();
                first.Flip(false);
                second.Flip(false);
            }

            _flippedCards.Clear();
            _isCheckingMatch = false;
        }
    }
}

