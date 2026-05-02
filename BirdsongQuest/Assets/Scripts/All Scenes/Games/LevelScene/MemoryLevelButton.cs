using Assets.Scripts.All_Scenes.Games;
using Assets.Scripts.All_Scenes.Games.FullMemoryGame;
using Assets.Scripts.All_Scenes.Games.FullMemoryGame.MemoryGame;
using Assets.Scripts.All_Scenes.Games.FullMemoryGame.Settings;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MemoryLevelButton : LevelButton
{
    [Header("Memory Settings")]
    [SerializeField] private MemorySettingsAnchor _settingsAnchor;
    [SerializeField] private MemoryLevel _memoryLevel;
    //[SerializeField] private MemoryMode _memoryMode;
    [SerializeField] private string _sceneName = "MemoryGame";

    protected override void Awake()
    {
        base.Awake();
        if (_settingsAnchor == null)
        {
            _settingsAnchor = Resources.Load<MemorySettingsAnchor>("Anchors/MemorySettingsAnchor");
            Debug.Log($"Anchor wurde automatisch geladen: {_settingsAnchor.name}");
        }
    }
    protected override void StartGame()
    {
        // 1️⃣ Settings erzeugen
        if (_settingsAnchor == null)
        {
            Debug.LogError("MemorySettingsAnchor ist nicht zugewiesen!");
            return;
        }

        // ScriptableObject mit aktuellen Level-Infos befüllen
        // 🧩 Neues Settings-Objekt erzeugen
        var settings = new MemorySettings(_memoryLevel, base._nextLevelAddsANewBird, _levelNumber);

        // 🧩 Im Anchor speichern
        _settingsAnchor.Item = settings;

        // 1️⃣ Session starten und Callback registrieren
        GameSessionManager.Instance.StartSession(OnGameFinished);

        // 2️⃣ Szene laden (Single oder Additive)
        SceneLoader.Instance.LoadSceneWithDelay(_sceneName, 0.5f);
    }
}

