using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.All_Scenes.Games.FullQuizGame.Settings;

namespace Assets.Scripts.All_Scenes.Games.FullQuizGame.QuizGameSettings
{
    public class QuizGameSettingsController : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TMP_Dropdown levelDropdown;
        [SerializeField] private QuizSettingsAnchor _settingsAnchor;

        public void OnStartGame()
        {
            QuizLevel level = levelDropdown.value switch
            {
                0 => QuizLevel.level1,
                1 => QuizLevel.level2,
                2 => QuizLevel.level3,
                3 => QuizLevel.level4,
                4 => QuizLevel.level5,
                5 => QuizLevel.level6,
                6 => QuizLevel.level7,
                _ => QuizLevel.level8
            };

            var settings = new QuizSettings(level, true,1);
            _settingsAnchor.Item = settings;    
        }

        public void Awake()
        {
            if (_settingsAnchor == null)
            {
                _settingsAnchor = Resources.Load<QuizSettingsAnchor>("Anchors/QuizSettingsAnchor");
                Debug.Log($"Anchor wurde automatisch geladen: {_settingsAnchor}");
            }
        }
    }
}
