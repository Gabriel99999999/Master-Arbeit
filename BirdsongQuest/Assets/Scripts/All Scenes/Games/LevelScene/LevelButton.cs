using Assets.Scripts.All_Scenes.Games;
using Assets.Scripts.All_Scenes.Games.LevelScene;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum GameType
{
    Memory,
    Quiz,
    BirdChor,
    // ... weitere Spiele hier hinzufügen
}

public abstract class LevelButton : MonoBehaviour
{
    [Header("Base Setup")]
    [SerializeField] protected int _levelNumber;
    [SerializeField] protected bool _isUnlocked;
    [SerializeField] protected string _gameName;
    [SerializeField] protected Button _button;
    [SerializeField] public LevelButtonGui _buttonGui;
    [SerializeField] protected InfoPanel _infoPanel;
    [SerializeField] protected LevelButton nextLevel;
    [SerializeField] protected GameObject nextPath;
    [SerializeField] protected bool _nextLevelAddsANewBird = true;

    public event Action<LevelButton> OnClicked;

    public void UnlockLevel()
    {
        _isUnlocked = true;
        _buttonGui?.SetActive(true);
    }

    protected virtual void Awake()
    {
        _button.onClick.AddListener(HandleClick);
        if (_button == null)
            _button = GetComponent<Button>();

        _isUnlocked = PlayerPrefs.GetInt($"UnlockedLevel") >= _levelNumber;
        _buttonGui?.SetActive(_isUnlocked);
        _button.interactable = _isUnlocked;
        if (PlayerPrefs.GetInt($"UnlockedLevel") > _levelNumber)
        {
            if(nextPath != null)
                UnlockPath();
        }
        _infoPanel?.Show(false);
    }
    private void HandleClick()
    {
        if (!_isUnlocked) return;
        OnClicked?.Invoke(this);
    }
    public void OnClick()
    {
        if (!_isUnlocked) 
            return;

        Debug.Log("Play button was clicked");
        string bestTime = GetBestTime();
        _infoPanel.SetInfo(_gameName, bestTime);
        _infoPanel.Show(true);
        _infoPanel.OnStartClicked = null;
        _infoPanel.OnStartClicked += StartGame;
        OnClicked?.Invoke(this);
    }

    protected string GetBestTime()
    {
        float time = PlayerPrefs.GetFloat($"LevelTime_{_levelNumber}", -1);
        if (time < 0) return "--:--";
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        return $"{minutes}:{seconds:00}";
    }

    protected void OnGameFinished(bool won)
    {
        if (!won) return;

        Debug.Log("Level geschafft!");
        
        if (PlayerPrefs.GetInt($"UnlockedLevel") == _levelNumber)
        {
            Debug.Log("Neues Level freigeschaltet!");
            string key = $"UnlockedLevel";
            PlayerPrefs.SetInt(key, _levelNumber+1);
            PlayerPrefs.Save();
        }
        
        _infoPanel.Show(false);
        if(nextPath != null)
            UnlockPath();
        if(nextLevel != null)
        {
            nextLevel.UnlockLevel();
            nextLevel._button.interactable = true;
        }
            
    }

    public void ClosePanel() => _infoPanel?.Show(false);
    public void OpenPanel() => _infoPanel?.Show(true);

    private void UnlockPath()
    {
        nextPath.GetComponent<Image>().color = new Color(205, 133, 63);
    }
    protected abstract void StartGame();
}