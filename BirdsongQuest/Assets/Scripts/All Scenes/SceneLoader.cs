using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    private string currentGameScene = string.Empty; // z. B. "MainMenu" oder "MemoryGame"
    private bool firstTimeOfPlaying = false;

    private void Awake()
    {
        //Unlock first level if no level is unlocked yet
        if(PlayerPrefs.GetInt("UnlockedLevel")==0)
        {
            firstTimeOfPlaying = true; 
        }
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private IEnumerator Start()
    {
        int width = 1920;   
        int height = 1080;
        FullScreenMode mode = FullScreenMode.FullScreenWindow;
        Screen.SetResolution(width, height, mode);


        // Wenn bereits Shared geladen ist, nicht erneut laden
        if (!SceneManager.GetSceneByName("Shared").isLoaded)
        {
            yield return SceneManager.LoadSceneAsync("Shared", LoadSceneMode.Additive);
        }

        //currentGameScene = "LevelScene";
        // Wenn bereits MainMenu aktiv ist, nichts tun
        if (firstTimeOfPlaying)
        {
            if (!SceneManager.GetSceneByName("Beginning").isLoaded)
            {
                yield return SceneManager.LoadSceneAsync("Beginning", LoadSceneMode.Additive);
            }
            currentGameScene = "Beginning";
        }
        else
        {
            if (!SceneManager.GetSceneByName("LevelScene").isLoaded)
            {
                yield return SceneManager.LoadSceneAsync("LevelScene", LoadSceneMode.Additive);
            }
        }

        var bootScene = SceneManager.GetSceneByName("BootScene");
        if (bootScene.IsValid() && bootScene.isLoaded)
        {
            Debug.Log("[SceneLoader] Entlade BootScene, Initialisierung abgeschlossen.");
            yield return SceneManager.UnloadSceneAsync(bootScene);
        }
    }

    private void OnEnable() => UIEvents.OnSceneLoadRequested += LoadSceneWithDelay;
    private void OnDisable() => UIEvents.OnSceneLoadRequested -= LoadSceneWithDelay;

    /// <summary>
    /// Spielt einen Sound und lädt danach eine neue Szene additiv.
    /// Alte Spielszene wird entladen, Shared bleibt bestehen.
    /// </summary>
    public void LoadSceneWithDelay(string sceneName, float delay = 0f)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName, delay));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName, float delay)
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);

        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("[SceneLoader] Kein Szenenname angegeben.");
            yield break;
        }

        // Alte Spielszene entladen, falls vorhanden
        if (!string.IsNullOrEmpty(currentGameScene) &&
            SceneManager.GetSceneByName(currentGameScene).isLoaded && currentGameScene != "LevelScene")
        {
            Debug.Log($"[SceneLoader] Entlade alte Spielszene: {currentGameScene}");
            yield return SceneManager.UnloadSceneAsync(currentGameScene);
        }

        // Neue Spielszene additiv laden
        if (!SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            Debug.Log($"[SceneLoader] Lade neue Spielszene additiv: {sceneName}");
            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            currentGameScene = sceneName;
        }
        else
        {
            Debug.Log("[SceneLoader] LevelScene bleibt aktiv – keine Neu-Ladung nötig.");
        }
    }
}
