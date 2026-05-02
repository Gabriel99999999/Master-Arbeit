using Assets.Scripts.All_Scenes.Games.FullMemoryGame;
using Assets.Scripts.All_Scenes.Games.FullMemoryGame.Settings;
using Assets.Scripts.All_Scenes.Games.FullQuizGame;
using Assets.Scripts.All_Scenes.Games.FullQuizGame.Settings;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuizLevelButton : LevelButton
{
    [Header("Quiz Settings")]
    [SerializeField] private QuizSettingsAnchor _settingsAnchor;
    [SerializeField] private QuizLevel _quizLevel;
    [SerializeField] private string _sceneName = "QuizGame";

    protected override void StartGame()
    {
        // 1️⃣ Settings erzeugen
        if (_settingsAnchor == null)
        {
            Debug.LogError("QuizSettingsAnchor ist nicht zugewiesen!");
            return;
        }
        var settings = new QuizSettings(_quizLevel, base._nextLevelAddsANewBird, _levelNumber);
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
            _settingsAnchor = Resources.Load<QuizSettingsAnchor>("Anchors/QuizSettingsAnchor");
            Debug.Log($"Anchor wurde automatisch geladen: {_settingsAnchor.name}");
        }
    }
}
