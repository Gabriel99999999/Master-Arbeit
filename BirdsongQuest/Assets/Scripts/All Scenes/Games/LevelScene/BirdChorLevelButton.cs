using Assets.Scripts.All_Scenes.Games.BirdChorGame;
using Assets.Scripts.All_Scenes.Games.FullMemoryGame;
using Assets.Scripts.All_Scenes.Games.FullMemoryGame.Settings;
using Assets.Scripts.All_Scenes.Games.FullQuizGame;
using Assets.Scripts.All_Scenes.Games.FullQuizGame.Settings;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BirdChorLevelButton : LevelButton
{
    [Header("BirdChor Settings")]
    [SerializeField] private BirdChorSettingsAnchor _settingsAnchor;
    [SerializeField] private BirdChorLevel _chorLevel;
    [SerializeField] private BirdChorMode _mode = BirdChorMode.synchronousSinging;
    [SerializeField] private string _sceneName = "BirdChorGame";

    protected override void StartGame()
    {
        // 1️⃣ Settings erzeugen
        if (_settingsAnchor == null)
        {
            Debug.LogError("BirdChorSettingsAnchor ist nicht zugewiesen!");
            return;
        }
        var settings = new BirdChorSettings(_chorLevel, _mode, base._nextLevelAddsANewBird, _levelNumber);
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
            _settingsAnchor = Resources.Load<BirdChorSettingsAnchor>("Anchors/BirdChorSettingsAnchor");
            Debug.Log($"Anchor wurde automatisch geladen: {_settingsAnchor.name}");
        }
    }
}
