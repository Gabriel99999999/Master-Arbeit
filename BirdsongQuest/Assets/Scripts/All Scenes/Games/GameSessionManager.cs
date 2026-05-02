using Assets.Scripts.All_Scenes.Games.LevelScene;
using System;
using UnityEngine;

public class GameSessionManager : MonoBehaviour
{
    [SerializeField] private Congratulationsscreen congratulationsScreen;
    public static GameSessionManager Instance { get; private set; }

    private Action<bool> _onGameFinished;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartSession(Action<bool> onGameFinished)
    {
        _onGameFinished = onGameFinished;
    }

    public void ReportResult(bool won, int level, Bird bird, bool nextLevelShowsANewBird)
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 0);
        //nur wenn das darauffolgende Level noch nicht freigeschalten war 
        if (won && level == unlockedLevel && nextLevelShowsANewBird)
        {
            congratulationsScreen.Activate(bird);
        }
        _onGameFinished?.Invoke(won);
        _onGameFinished = null; // aufräumen
    }
}
