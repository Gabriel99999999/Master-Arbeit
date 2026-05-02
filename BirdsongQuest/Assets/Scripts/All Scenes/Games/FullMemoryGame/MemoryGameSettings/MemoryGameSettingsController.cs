using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.All_Scenes.Games.FullMemoryGame.Settings;

namespace Assets.Scripts.All_Scenes.Games.FullMemoryGame.MemoryGameSettings
{
    public class MemoryGameSettingsController : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TMP_Dropdown levelDropdown;
        [SerializeField] private Toggle toggleNameAndImage;
        [SerializeField] private Toggle toggleSoundAndImage;

        [SerializeField] private MemorySettingsAnchor _settingsAnchor;

        public void Awake()
        {
            if (_settingsAnchor == null)
            {
                _settingsAnchor = Resources.Load<MemorySettingsAnchor>("Anchors/MemorySettingsAnchor");
                Debug.Log($"Anchor wurde automatisch geladen: {_settingsAnchor}");
            }
        }
        
        public void OnStartGame()
        {
            Debug.Log("[MemoryGameSettingsController] Starte Memory Game …");

            //MemoryMode mode = toggleNameAndImage.isOn ? MemoryMode.NameAndImage : MemoryMode.SoundAndImage;
            MemoryLevel level = levelDropdown.value switch
            {
                0 => MemoryLevel.level1,
                1 => MemoryLevel.level2,
                2 => MemoryLevel.level3,
                3 => MemoryLevel.level4,
                4 => MemoryLevel.level5,
                5 => MemoryLevel.level6,
                6 => MemoryLevel.level7,
                _ => MemoryLevel.level8
            };

            var settings = new MemorySettings(level, true,1);
            _settingsAnchor.Item = settings;
        }
    }
}
