using Assets.Scripts.All_Scenes.Games.BirdChorGame;
using Assets.Scripts.All_Scenes.Games.FindeDieAmsel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.All_Scenes.Games.LevelScene
{
    public class FindeDieAmselLevelButton : LevelButton
    {
        [Header("BirdChor Settings")]
        [SerializeField] private FindeDieAmselSettingsAnchor _settingsAnchor;
        [SerializeField] private FindeDieAmselLevel _findeDieAmselLevel;
        [SerializeField] private string _sceneName = "FindeDieAmsel";
        

        protected override void StartGame()
        {
            // 1️⃣ Settings erzeugen
            if (_settingsAnchor == null)
            {
                Debug.LogError("BirdChorSettingsAnchor ist nicht zugewiesen!");
                return;
            }
            var settings = new FindeDieAmselSettings(_findeDieAmselLevel, base._nextLevelAddsANewBird, _levelNumber);
            _settingsAnchor.Item = settings;

            // 1️⃣ Session starten und Callback registrieren
            GameSessionManager.Instance.StartSession(OnGameFinished);

            // 2️⃣ Szene laden (Single oder Additive)
            SceneLoader.Instance.LoadSceneWithDelay(_sceneName, 0.5f);
        }

        protected override void Awake()
        {
            base.Awake();
            if (_settingsAnchor == null)
            {
                _settingsAnchor = Resources.Load<FindeDieAmselSettingsAnchor>("Anchors/FindeDieAmselSettingsAnchor");
                Debug.Log($"Anchor wurde automatisch geladen: {_settingsAnchor.name}");
            }
        }
    }
}
