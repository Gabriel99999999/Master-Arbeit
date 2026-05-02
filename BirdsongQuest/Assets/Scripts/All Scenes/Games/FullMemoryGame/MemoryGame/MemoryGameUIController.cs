using Assets.Scripts.Timer;
using Assets.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.All_Scenes.Games.FullMemoryGame.MemoryGame
{
    public class MemoryGameUIController : MonoBehaviour
    {
        [SerializeField] private TMP_Text _pauseButtonText;
        [SerializeField] private GameObject _pauseMenu;
        [SerializeField] private TextMeshProUGUI _level;
        [SerializeField] private TextMeshProUGUI _moveCounter;
        [SerializeField] private TextMeshProUGUI _pairs;
        [SerializeField] private GridBuilder _gridBuilder;

        public event Action OnPauseButtonClicked;
        public event Action OnEndButtonClicked;

        public void OnPauseOrPlay()
        {
            OnPauseButtonClicked?.Invoke();
        }
        public void SetPauseMenuActive(bool isPaused)
        {
            _pauseMenu.SetActive(isPaused);
            _pauseButtonText.text = isPaused ? "Play" : "Pause";
        }
        public void OnEndGame()
        {
            OnEndButtonClicked?.Invoke();
        }
        public void UpdateLevelText(int level)
        {
            // könnte später noch animiert oder gestylt werden
            Debug.Log($"Level: {level}");
            _level.text = level.ToString();
        }

        public void UpdatePairText(int found, int total)
        {
            Debug.Log($"Pairs: {found}/{total}");
            _pairs.text = $"{found}/{total}";
        }

        public void UpdateMoveCounter(int moves)
        {
            Debug.Log($"Moves: {moves}");
            _moveCounter.text = moves.ToString();
        }

        public void CreateBoard(List<(Bird bird, CardType type)> cards)
        {
            _gridBuilder.BuildGrid(cards);
        }
    }
}
